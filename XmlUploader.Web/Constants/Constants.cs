﻿namespace XmlUploader.Web.Constants
{
    public static class Constants
    {

        public static string FileApiUrl { get; set; }

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }

        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
