using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DahuaCameraTestApp
{
    internal sealed class DahuaSdkClient : IDisposable
    {
        private const string ManagedSdkFileName = "DHNetSDKCS.dll";

        private readonly string[] _requiredRuntimeFiles =
        {
            "DHNetSDKCS.dll",
            "dhnetsdk.dll",
            "dhconfigsdk.dll",
            "dhplay.dll",
            "dhlog.dll"
        };

        private Assembly _sdkAssembly;
        private Type _dhClientType;
        private Type _realPlayTypeEnum;
        private Type _languageEncodingEnum;
        private Type _deviceInfoType;
        private bool _initialized;
        private int _loginHandle;
        private int _previewHandle;

        public string LoadedSdkAssemblyPath { get; private set; }

        public string LastErrorText { get; private set; }

        public bool IsInitialized
        {
            get { return _initialized; }
        }

        public bool IsLoggedIn
        {
            get { return _loginHandle != 0; }
        }

        public bool HasActivePreview
        {
            get { return _previewHandle != 0; }
        }

        public int LoginHandle
        {
            get { return _loginHandle; }
        }

        public int PreviewHandle
        {
            get { return _previewHandle; }
        }

        public bool Init(Action<string> log)
        {
            try
            {
                if (_initialized)
                {
                    logMessage(log, "SDK already initialized.");
                    return true;
                }

                if (!TryLoadSdk(log))
                    return false;

                InvokeOptional("DHSetShowException", false);
                InvokeOptional("DHSetConnectTime", 3000, 1);

                bool initOk = Convert.ToBoolean(InvokeRequired("DHInit", null, IntPtr.Zero));
                _initialized = initOk;
                logMessage(log, string.Format("[{0}] SDK init {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), initOk ? "SUCCESS" : "FAIL"));

                if (_initialized)
                    TrySetEncodingGb2312(log);
                else
                    LastErrorText = "DHInit FAIL | sdkLastError=" + GetLastError();

                return _initialized;
            }
            catch (Exception ex)
            {
                CaptureException("Init SDK failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        public bool Login(string ip, ushort port, string username, string password, Action<string> log)
        {
            try
            {
                if (!Init(log))
                    return false;

                Logout(log);

                object deviceInfo = Activator.CreateInstance(_deviceInfoType);
                object[] args = { ip, port, username, password, deviceInfo, 0 };
                int loginHandle = Convert.ToInt32(InvokeRequired("DHLogin", args));
                _loginHandle = loginHandle;

                if (_loginHandle == 0)
                {
                    int lastError = GetLastError();
                    LastErrorText = string.Format(
                        "Login FAIL | ip={0} port={1} user={2} lastError={3}",
                        ip,
                        port,
                        username,
                        lastError);
                    logMessage(log, LastErrorText);
                    return false;
                }

                logMessage(
                    log,
                    string.Format(
                        "Login SUCCESS | ip={0} port={1} user={2} loginHandle={3}",
                        ip,
                        port,
                        username,
                        _loginHandle));
                return true;
            }
            catch (Exception ex)
            {
                CaptureException("Login failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        public bool StartPreview(int channel, IntPtr previewHandle, Action<string> log)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    LastErrorText = "StartPreview FAIL | not logged in.";
                    logMessage(log, LastErrorText);
                    return false;
                }

                if (previewHandle == IntPtr.Zero)
                {
                    LastErrorText = "StartPreview FAIL | preview window handle is zero.";
                    logMessage(log, LastErrorText);
                    return false;
                }

                StopPreview(log);

                int sdkChannel = Math.Max(0, channel - 1);
                object streamType = GetDefaultRealPlayTypeValue();
                object[] playExArgs = { _loginHandle, sdkChannel, streamType, previewHandle };
                _previewHandle = Convert.ToInt32(InvokeOptional("DHRealPlayEx", playExArgs));

                if (_previewHandle == 0)
                {
                    object[] fallbackArgs = { _loginHandle, sdkChannel, previewHandle };
                    _previewHandle = Convert.ToInt32(InvokeOptional("DHRealPlay", fallbackArgs));
                }

                int error = GetLastError();
                bool ok = _previewHandle != 0;
                string message = string.Format(
                    "StartPreview {0} | configChannel={1} sdkChannel={2} hwnd={3} previewHandle={4} sdkLastError={5}",
                    ok ? "SUCCESS" : "FAIL",
                    channel,
                    sdkChannel,
                    previewHandle,
                    _previewHandle,
                    error);
                LastErrorText = message;
                logMessage(log, message);
                return ok;
            }
            catch (Exception ex)
            {
                CaptureException("StartPreview failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        public bool StopPreview(Action<string> log)
        {
            try
            {
                if (_previewHandle == 0)
                    return true;

                bool stopOk = Convert.ToBoolean(InvokeOptional("DHStopRealPlayEx", _previewHandle));
                if (!stopOk)
                    stopOk = Convert.ToBoolean(InvokeOptional("DHStopRealPlay", _previewHandle));

                int lastHandle = _previewHandle;
                int lastError = GetLastError();
                _previewHandle = 0;
                string message = string.Format(
                    "StopPreview {0} | previewHandle={1} sdkLastError={2}",
                    stopOk ? "SUCCESS" : "FAIL",
                    lastHandle,
                    lastError);
                LastErrorText = message;
                logMessage(log, message);
                return stopOk;
            }
            catch (Exception ex)
            {
                CaptureException("StopPreview failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        public bool CaptureSnapshot(string snapshotPath, Action<string> log)
        {
            try
            {
                if (_previewHandle == 0)
                {
                    LastErrorText = "CaptureSnapshot FAIL | preview is not active.";
                    logMessage(log, LastErrorText);
                    return false;
                }

                string folder = Path.GetDirectoryName(snapshotPath);
                if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                bool ok = Convert.ToBoolean(InvokeRequired("DHCapturePicture", _previewHandle, snapshotPath));
                int lastError = GetLastError();
                string message = string.Format(
                    "CaptureSnapshot {0} | previewHandle={1} path={2} sdkLastError={3}",
                    ok ? "SUCCESS" : "FAIL",
                    _previewHandle,
                    snapshotPath,
                    lastError);
                LastErrorText = message;
                logMessage(log, message);
                return ok;
            }
            catch (Exception ex)
            {
                CaptureException("CaptureSnapshot failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        public void Logout(Action<string> log)
        {
            try
            {
                StopPreview(log);
                if (_loginHandle == 0)
                    return;

                int currentLoginHandle = _loginHandle;
                InvokeOptional("DHLogout", _loginHandle);
                _loginHandle = 0;
                logMessage(log, string.Format("Logout SUCCESS | loginHandle={0}", currentLoginHandle));
            }
            catch (Exception ex)
            {
                CaptureException("Logout failed", ex);
                logMessage(log, LastErrorText);
            }
        }

        public void Cleanup(Action<string> log)
        {
            try
            {
                Logout(log);
                if (!_initialized)
                    return;

                InvokeOptional("DHCleanup");
                _initialized = false;
                logMessage(log, "SDK cleanup SUCCESS.");
            }
            catch (Exception ex)
            {
                CaptureException("Cleanup failed", ex);
                logMessage(log, LastErrorText);
            }
        }

        public int GetLastError()
        {
            try
            {
                if (_dhClientType == null)
                    return -1;

                MethodInfo method = _dhClientType.GetMethod("DHGetLastError", BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                    return -1;

                return Convert.ToInt32(method.Invoke(null, null));
            }
            catch
            {
                return -1;
            }
        }

        public string GetSdkSearchSummary()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string libsDir = Path.Combine(baseDir, "libs");

            var lines = new List<string>
            {
                "SDK search paths:",
                " - " + baseDir,
                " - " + libsDir
            };

            IEnumerable<string> missingFiles = _requiredRuntimeFiles.Where(file => !TryResolveRuntimeFile(file, out _));
            foreach (string missing in missingFiles)
                lines.Add("Missing: " + missing);

            return string.Join(Environment.NewLine, lines);
        }

        public void Dispose()
        {
            Cleanup(null);
        }

        private bool TryLoadSdk(Action<string> log)
        {
            if (_sdkAssembly != null && _dhClientType != null)
                return true;

            if (!TryResolveRuntimeFile(ManagedSdkFileName, out string managedSdkPath))
            {
                LastErrorText = "SDK load FAIL | Missing " + ManagedSdkFileName + Environment.NewLine + GetSdkSearchSummary();
                logMessage(log, LastErrorText);
                return false;
            }

            try
            {
                LoadedSdkAssemblyPath = managedSdkPath;
                _sdkAssembly = Assembly.LoadFrom(managedSdkPath);
                _dhClientType = _sdkAssembly.GetType("DHNetSDK.DHClient", true);
                _realPlayTypeEnum = _sdkAssembly.GetType("DHNetSDK.REALPLAY_TYPE", false);
                _languageEncodingEnum = _sdkAssembly.GetType("DHNetSDK.LANGUAGE_ENCODING", false);
                _deviceInfoType = _sdkAssembly.GetType("DHNetSDK.NET_DEVICEINFO", true);
                logMessage(log, "SDK assembly loaded: " + managedSdkPath);
                return true;
            }
            catch (Exception ex)
            {
                CaptureException("SDK load failed", ex);
                logMessage(log, LastErrorText);
                return false;
            }
        }

        private bool TryResolveRuntimeFile(string fileName, out string fullPath)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(baseDir, fileName),
                Path.Combine(baseDir, "libs", fileName)
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    fullPath = candidate;
                    return true;
                }
            }

            fullPath = null;
            return false;
        }

        private void TrySetEncodingGb2312(Action<string> log)
        {
            try
            {
                if (_languageEncodingEnum == null)
                    return;

                object gb2312Value = Enum.Parse(_languageEncodingEnum, "gb2312", true);
                InvokeOptional("DHSetEncoding", gb2312Value);
                logMessage(log, "SDK encoding set to gb2312.");
            }
            catch (Exception ex)
            {
                logMessage(log, "SDK encoding setup skipped: " + ex.Message);
            }
        }

        private object GetDefaultRealPlayTypeValue()
        {
            if (_realPlayTypeEnum == null)
                return null;

            Array values = Enum.GetValues(_realPlayTypeEnum);
            if (values.Length == 0)
                return null;

            return values.GetValue(0);
        }

        private object InvokeRequired(string methodName, params object[] args)
        {
            MethodInfo method = _dhClientType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new MissingMethodException(_dhClientType.FullName, methodName);

            return method.Invoke(null, args);
        }

        private object InvokeOptional(string methodName, params object[] args)
        {
            MethodInfo method = _dhClientType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                return null;

            return method.Invoke(null, args);
        }

        private void CaptureException(string prefix, Exception ex)
        {
            LastErrorText = string.Format(
                "{0} | {1}: {2}{3}{4}",
                prefix,
                ex.GetType().FullName,
                ex.Message,
                Environment.NewLine,
                ex.StackTrace);
        }

        private static void logMessage(Action<string> log, string message)
        {
            if (log == null || string.IsNullOrWhiteSpace(message))
                return;

            log(message);
        }
    }
}
