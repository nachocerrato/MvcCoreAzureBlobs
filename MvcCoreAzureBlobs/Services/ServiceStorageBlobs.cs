using Azure.Storage.Blobs;
using MvcCoreAzureBlobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAzureBlobs.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(string keys)
        {
            this.client = new BlobServiceClient(keys);
        }

        //Método para devolver todos los contenedores
        public async Task<List<string>> GetContainerAsync()
        {
            List<string> containers = new List<string>();
            await foreach (var container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        //Método para crear nuevos contenedores
        public async Task CreateContainerAsync(string nombre)
        {
            await this.client.CreateBlobContainerAsync(nombre.ToLower(),
                Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }

        public async Task DeleteContainerAsync(string nombre)
        {
            await this.client.DeleteBlobContainerAsync(nombre);
        }

        //Método para mostrar los blobs de un contenedor
        public async Task<List<Blob>> GetBlobAsync(string containerName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<Blob> blobs = new List<Blob>();
            await foreach(var blob in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                blobs.Add(
                    new Blob { Nombre = blob.Name,
                    Url = blobClient.Uri.AbsoluteUri});
            }
            return blobs;
        }

        //Método para eliminar un blob
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        //Método para subir un blob a azure
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
