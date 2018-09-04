using System;
using System.IO;
using System.Net;

namespace WikiQueries{
    public class Wiki{

        private static string BASE_URI = "https://en.wikipedia.org/w/api.php";
        
        public static string QueryPageName(QueryBuilder query){
                     
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query.ToString());//
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            if (stream == null){
                throw new QueryResponseException("Failed to retrieve a response for query: " + query);
            }
            
            string result;
            using(StreamReader sr = new StreamReader(stream)){
                result = sr.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// Grab the first paragraph of a page
        /// </summary>
        /// <param name="pageTitle">Title of wiki page</param>
        /// <param name="format">Format to retrieve</param>
        /// <param name="plainText">Retrieve as plain text or with html tags</param>
        /// <returns></returns>
        public static string PageSummary(string pageTitle, Format format=Format.XML, bool plainText=false){
            QueryBuilder query = new QueryBuilder(BASE_URI, format);
            
            query.AppendParam("action", "query");
            query.AppendParam("titles", pageTitle);
            query.AppendParam("prop", "extracts");
            query.AppendParam("exintro");
            if (plainText){
                query.AppendParam("explaintext");
            }

            query.Format = Format.XML;
            string result = QueryPageName(query);
            return result;
        }

        /// <summary>
        /// Check a page exists
        /// </summary>
        /// <param name="pageTitle">Title of wiki page</param>
        /// <returns></returns>
        public static bool DoesPageExist(string pageTitle){
            QueryBuilder query = new QueryBuilder(BASE_URI, Format.JSON);
            
            query.AppendParam("action", "query");
            query.AppendParam("titles", pageTitle);
            query.Format = Format.XML;
            string result = QueryPageName(query);
            
            ParserXml parserXml = new ParserXml(result);
            string idStr = parserXml.PageId();
            try{
                int id = Int32.Parse(idStr);
                if (id >= 0){
                    return true;
                }
            }
            catch(FormatException e){
                return false;
            }
            return false;
        }

        public static string BaseUri => BASE_URI;
    }

    class QueryResponseException : Exception{
        public QueryResponseException(string message) : base(message){
        }
    }
}