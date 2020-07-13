// <copyright file="AssetLibraryController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for asset library
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("AssetLibrary")]
    public class AssetLibraryController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private AssetLibraryManager assetLibraryManager = null;

        #endregion

        #region Constructor

        public AssetLibraryController(IUnityContainer unityContainer)
        {
            this.assetLibraryManager = new AssetLibraryManager(unityContainer);
        }

        #endregion

        #region Public Methods

        #region Asset Library

        /// <summary>
        /// This method helps to add asset libraries
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<AssetLibrary> assetLibraries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.AddAssetLibraries(assetLibraries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update assetLibraries.
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<AssetLibrary> assetLibraries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.UpdateAssetLibraries(assetLibraries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete asset libraries.
        /// </summary>
        /// <param name="asset libraries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<AssetLibrary> assetLibraries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.DeleteAssetLibraries(assetLibraries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="assetLibrarySearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<AssetLibrary> List(AssetLibrarySearchParameter assetLibrarySearchParameter)
        {
            IList<AssetLibrary> assetlibraries = new List<AssetLibrary>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                assetlibraries = this.assetLibraryManager.GetAssetLibraries(assetLibrarySearchParameter, tenantCode);
                if (assetlibraries?.Count > 0 && assetLibrarySearchParameter.IsAssetDataRequired)
                {
                    string path = HttpContext.Current.Server.MapPath("~") + ModelConstant.ASSETPATHSLASH + ModelConstant.ASSETS;
                    assetlibraries.ToList().ForEach(library =>
                    {
                        if (library.Assets?.Count() > 0)
                            library.Assets.ToList().ForEach(asset =>
                            {
                                asset.FilePath = path + ModelConstant.ASSETPATHSLASH + library.Identifier + ModelConstant.ASSETPATHSLASH + asset.Name;
                            });
                    });
                }
                HttpContext.Current.Response.AppendHeader("recordCount", this.assetLibraryManager.GetAssetLibraryCount(assetLibrarySearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assetlibraries;
        }

        /// <summary>
        /// This method helps to get asset library based on given identifier.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>asset library record</returns>
        [HttpGet]
        public AssetLibrary Detail(long assetLibraryIdentifier)
        {
            IList<AssetLibrary> assetLibraries = new List<AssetLibrary>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                AssetLibrarySearchParameter assetLibrarySearchParameter = new AssetLibrarySearchParameter();
                assetLibrarySearchParameter.Identifier = assetLibraryIdentifier.ToString();
                assetLibrarySearchParameter.IsAssetDataRequired = true;
                assetLibrarySearchParameter.SortParameter.SortColumn = "Id";
                assetLibraries = this.assetLibraryManager.GetAssetLibraries(assetLibrarySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assetLibraries.First();
        }




        #endregion

        #region Public Assets Methods

        #region Asset Crud Operation

        /// <summary>
        /// This method helps to add asset s
        /// </summary>
        /// <param name="assets"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        [Route("Asset/Add")]
        public bool AddAsset(IList<Asset> assets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.AddAssets(assets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update assets.
        /// </summary>
        /// <param name="assets"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        [Route("Asset/Update")]
        public bool UpdateAsset(IList<Asset> assets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.UpdateAssets(assets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete asset s.
        /// </summary>
        /// <param name="asset s"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        [Route("Asset/Delete")]
        public bool DeleteAsset(IList<Asset> assets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.DeleteAssets(assets, tenantCode);

            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get asset s list based on the search parameters.
        /// </summary>
        /// <param name="assetSearchParameter"></param>
        /// <returns>List of asset s</returns>
        [HttpPost]
        [Route("Asset/List")]
        public IList<Asset> GetAsset(AssetSearchParameter assetSearchParameter)
        {
            IList<Asset> assets = new List<Asset>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                assets = this.assetLibraryManager.GetAssets(assetSearchParameter, tenantCode);
                string path = HttpContext.Current.Server.MapPath("~") + ModelConstant.ASSETPATHSLASH + ModelConstant.ASSETS;


                assets?.ToList().ForEach(asset =>
                {
                    asset.FilePath = path + ModelConstant.ASSETPATHSLASH + asset.AssetLibraryIdentifier + ModelConstant.ASSETPATHSLASH + asset.Name;
                });

                HttpContext.Current.Response.AppendHeader("recordCount", this.assetLibraryManager.GetAssetCount(assetSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assets;
        }

        /// <summary>
        /// This method helps to get asset  based on given identifier.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>asset  record</returns>
        [HttpGet]
        [Route("Asset/Detail")]
        public Asset AssetDetail(long assetIdentifier)
        {
            IList<Asset> assets = new List<Asset>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                AssetSearchParameter assetSearchParameter = new AssetSearchParameter();
                assetSearchParameter.Identifier = assetIdentifier.ToString();
                assetSearchParameter.SortParameter.SortColumn = "Id";
                assets = this.assetLibraryManager.GetAssets(assetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assets.First();
        }


        /// <summary>
        /// This method refence to add asset path
        /// </summary>
        /// <param name="assetPathSetting"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Asset/AddAssetPath")]
        public bool AddAssetPath(AssetPathSetting assetPathSetting)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.assetLibraryManager.AddAssetPath(assetPathSetting, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Get asset storage path
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Asset/GetAssetPath")]
        public AssetPathSetting GetAssetPath()
        {
            string tenantCode = Helper.CheckTenantCode(Request.Headers);
            return this.assetLibraryManager.GetAssetPath(tenantCode);
        }

        #endregion

        #region Upload Assets

        [HttpPost]
        [Route("Asset/Upload")]
        public bool Upload()
        {
            try
            {
                bool uploadStatus = false;
                bool resultStatus = false;
                string fileName = string.Empty;
                var filePath = string.Empty;
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                long assetLibraryIdentifier = 0;
                bool isFolderUpload = false;
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;


                if (httpRequest.Form.GetValues(ModelConstant.ASSET_LIBRARY_IDENTIFIER).FirstOrDefault() == string.Empty
                    || httpRequest.Form.GetValues(ModelConstant.ASSET_LIBRARY_IDENTIFIER).FirstOrDefault() == "0"
                    )
                {
                    throw new InvalidAssetLibraryException(tenantCode);
                }
                if (httpRequest.Form.GetValues("LastUpdatedBy").FirstOrDefault() == string.Empty
                   || httpRequest.Form.GetValues("LastUpdatedBy").FirstOrDefault() == "0"
                   )
                {
                    throw new UserNotFoundException(tenantCode);
                }

                assetLibraryIdentifier = long.Parse(httpRequest.Form.GetValues(ModelConstant.ASSET_LIBRARY_IDENTIFIER).FirstOrDefault());
                isFolderUpload = bool.Parse(httpRequest.Form.GetValues("IsFolderUpload").FirstOrDefault());
                long lastUpdatedBy = long.Parse(httpRequest.Form.GetValues("LastUpdatedBy").FirstOrDefault());

                if (httpRequest.Files.Count > 0)
                {
                    int count = 0;
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        IList<Asset> assets = new List<Asset>();

                        var postedFile = httpRequest.Files[count];

                        string imagePath = string.Empty;
                        imagePath = HttpContext.Current.Server.MapPath("~");

                        //var assetPathSetting = assetLibraryManager.GetAssetPath(tenantCode);
                        //if (assetPathSetting == null || string.IsNullOrWhiteSpace(assetPathSetting.AssetPath))
                        //{
                        //    throw new AssetPathNotFoundException(tenantCode);
                        //}
                        //imagePath = assetPathSetting.AssetPath;

                        var customerPath = imagePath + ModelConstant.ASSETPATHSLASH;

                        if (!Directory.Exists(customerPath))
                        {
                            //If No any such directory then creates the new one based on tenant code
                            Directory.CreateDirectory(customerPath);
                        }
                        var assetPath = customerPath + ModelConstant.ASSETS + ModelConstant.ASSETPATHSLASH + assetLibraryIdentifier + ModelConstant.ASSETPATHSLASH;

                        if (!Directory.Exists(assetPath))
                        {
                            //If No any such directory then creates the new one based on tenant code
                            Directory.CreateDirectory(assetPath);
                        }
                        fileName = postedFile.FileName;

                        if (isFolderUpload)
                        {
                            fileName = fileName.Split('/')[1];  // demo/sampleDMP4.mp4
                        }
                        filePath = assetPath + fileName;

                        //bool isAllowAssetLibraryReplace = this.assetLibraryManager.AllowAssetLibraryReplace(assetLibraryIdentifier.ToString(), tenantCode, fileName);
                        //if (!isAllowAssetLibraryReplace)
                        //{
                        //    throw new AssetUsedInCapmaignException(tenantCode);
                        //}

                        //if (File.Exists(filePath) && !isAllowAssetLibraryReplace)
                        //{
                        //    //Disscussion pending
                        //    throw new DuplicateAssetException(tenantCode);
                        //}
                        //bool isAllowAssetLibraryReplace = true;
                        //if (isAllowAssetLibraryReplace)
                        //{
                        var items = postedFile.FileName.Split('.');

                        string fileExtension = items[items.Length - 1];
                        fileName=fileName.Replace(fileExtension, fileExtension.ToLower());
                        assets.Add(new Asset()
                        {
                            Name = fileName,
                            FilePath = fileName,
                            AssetLibraryIdentifier = assetLibraryIdentifier,
                            LastUpdatedBy = new User { Identifier = lastUpdatedBy },
                            LastUpdatedDate = DateTime.UtcNow
                        }); ;
                        this.assetLibraryManager.AddAssets(assets, tenantCode);
                        //}

                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);

                        uploadStatus = true;

                        count++;
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
                    resultStatus = true;
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                //if (resultStatus == true)
                //{
                //    if (assets?.Count > 0)
                //    {
                //        this.assetLibraryManager.AddAssets(assets, tenantCode);
                //        uploadStatus = true;
                //    }
                //}

                return uploadStatus;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [HttpPost]
        [Route("Asset/AddSSML")]
        public bool AddSSMLAsset(Asset asset)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                IList<Asset> assets = new List<Asset>();
                string imagePath = string.Empty;
                var assetPathSetting = assetLibraryManager.GetAssetPath(tenantCode);
                if (assetPathSetting == null || string.IsNullOrWhiteSpace(assetPathSetting.AssetPath))
                {
                    throw new AssetPathNotFoundException(tenantCode);
                }

                imagePath = assetPathSetting.AssetPath;

                var customerPath = imagePath + ModelConstant.ASSETPATHSLASH + tenantCode + ModelConstant.ASSETPATHSLASH;

                if (!Directory.Exists(customerPath))
                {
                    //If No any such directory then creates the new one based on tenant code
                    Directory.CreateDirectory(customerPath);
                }
                var assetPath = customerPath + ModelConstant.ASSETS + ModelConstant.ASSETPATHSLASH + asset.AssetLibraryIdentifier + ModelConstant.ASSETPATHSLASH;

                if (!Directory.Exists(assetPath))
                {
                    //If No any such directory then creates the new one based on tenant code
                    Directory.CreateDirectory(assetPath);
                }
                string fileName = asset.Name;
                string filePath = assetPath + fileName;
                File.WriteAllText(filePath, asset.FileContent);

                assets.Add(new Asset()
                {
                    Name = fileName,
                    FilePath = filePath,
                    AssetLibraryIdentifier = asset.AssetLibraryIdentifier
                });

                result = this.assetLibraryManager.AddAssets(assets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        [HttpPost]
        [Route("Asset/UpdateSSML")]
        public bool UpdateSSMLAsset(Asset asset)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                IList<Asset> assets = new List<Asset>();
                string imagePath = string.Empty;
                var assetPathSetting = assetLibraryManager.GetAssetPath(tenantCode);
                if (assetPathSetting == null || string.IsNullOrWhiteSpace(assetPathSetting.AssetPath))
                {
                    throw new AssetPathNotFoundException(tenantCode);
                }

                imagePath = assetPathSetting.AssetPath;

                var customerPath = imagePath + ModelConstant.ASSETPATHSLASH + tenantCode + ModelConstant.ASSETPATHSLASH;

                if (!Directory.Exists(customerPath))
                {
                    //If No any such directory then creates the new one based on tenant code
                    Directory.CreateDirectory(customerPath);
                }
                var assetPath = customerPath + ModelConstant.ASSETS + ModelConstant.ASSETPATHSLASH + asset.AssetLibraryIdentifier + ModelConstant.ASSETPATHSLASH;

                if (!Directory.Exists(assetPath))
                {
                    //If No any such directory then creates the new one based on tenant code
                    Directory.CreateDirectory(assetPath);
                }
                string fileName = asset.Name;
                string filePath = assetPath + fileName;
                File.WriteAllText(filePath, asset.FileContent);

                assets.Add(new Asset()
                {
                    Identifier = asset.Identifier,
                    Name = fileName,
                    FilePath = filePath,
                    AssetLibraryIdentifier = asset.AssetLibraryIdentifier
                });

                result = this.assetLibraryManager.UpdateAssets(assets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #region Upload R&D commented

        //[HttpPost]
        //public async Task<HttpResponseMessage> PostFile()
        //{
        //    // Check if the request contains multipart/form-data. 
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string root = HttpContext.Current.Server.MapPath("~/App_Data");
        //    var provider = new MultipartFormDataStreamProvider(root);

        //    try
        //    {
        //        StringBuilder sb = new StringBuilder(); // Holds the response body 

        //        // Read the form data and return an async task. 
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        // This illustrates how to get the form data. 
        //        foreach (var key in provider.FormData.AllKeys)
        //        {
        //            foreach (var val in provider.FormData.GetValues(key))
        //            {
        //                sb.Append(string.Format("{0}: {1}\n", key, val));
        //            }
        //        }

        //        // This illustrates how to get the file names for uploaded files. 
        //        foreach (var file in provider.FileData)
        //        {
        //            FileInfo fileInfo = new FileInfo(file.LocalFileName);
        //            sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
        //        }
        //        return new HttpResponseMessage()
        //        {
        //            Content = new StringContent(sb.ToString())
        //        };
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}

        #endregion

        #endregion

        #region Download

        [HttpGet]
        [Route("Asset/Download")]
        public HttpResponseMessage Download(string assetIdentifier)
        {
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                string path = string.Empty;
                Asset asset = this.assetLibraryManager.GetAssets(new AssetSearchParameter()
                {
                    Identifier = assetIdentifier,
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();

                string relativePath = HttpContext.Current.Server.MapPath("~") + ModelConstant.ASSETPATHSLASH + ModelConstant.ASSETS;

                asset.FilePath = relativePath + ModelConstant.ASSETPATHSLASH + asset.AssetLibraryIdentifier + ModelConstant.ASSETPATHSLASH + asset.Name;


                if (asset == null)
                {
                    throw new AssetNotFoundException(tenantCode);
                }

                path = asset.FilePath.ToString();

                if (!File.Exists(path))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", asset.Name);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = asset.Name;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
                catch (IOException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Asset/Preview")]
        public HttpResponseMessage Preview(string assetIdentifier)
        {
            string tenantCode = Helper.CheckTenantCode(Request.Headers);
            string path = string.Empty;
            Asset asset = this.assetLibraryManager.GetAssets(new AssetSearchParameter()
            {
                Identifier = assetIdentifier,
                SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
            }, tenantCode).FirstOrDefault();

            string relativePath = HttpContext.Current.Server.MapPath("~") + ModelConstant.ASSETPATHSLASH + ModelConstant.ASSETS;

            asset.FilePath = relativePath + ModelConstant.ASSETPATHSLASH + asset.AssetLibraryIdentifier + ModelConstant.ASSETPATHSLASH + asset.Name;


            if (asset == null)
            {
                throw new AssetNotFoundException(tenantCode);
            }

            path = asset.FilePath.ToString();

            if (!File.Exists(path))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);

                        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                        httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                        httpResponseMessage.Content.Headers.Add("x-filename", asset.Name);
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = asset.Name;
                        httpResponseMessage.StatusCode = HttpStatusCode.OK;
                        return httpResponseMessage;
                    }
                }
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

        #endregion

        #endregion

        #endregion

    }
}