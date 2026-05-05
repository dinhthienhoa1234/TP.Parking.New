using System;
using System.Collections.Generic;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;

namespace THPARKING.Data.InMemory
{
    public class InMemoryParkingImageRepository : IParkingImageRepository
    {
        private readonly IList<ParkingImage> _images;

        public InMemoryParkingImageRepository(IList<ParkingImage> images)
        {
            _images = images ?? throw new ArgumentNullException(nameof(images));
        }

        public void Add(ParkingImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            _images.Add(image);
        }
    }
}
