using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AluraChallengeKarina.Models;
using Newtonsoft.Json;

namespace AluraChallengeKarina.Controllers
{
    public class VideosController : ApiController
    {
        public SqlConnection ConnectionString { get; set; }

        public VideosController()
        {
            ConnectionString = new SqlConnection(ConfigurationManager.ConnectionStrings["connection-challenge"].ConnectionString);
        }

        public List<Video> Get()
        {
            using (ConnectionString)
            {
                ConnectionString.Open();
                string selecionaTudo = "SELECT * FROM VIDEO";

                SqlCommand cmd = new SqlCommand(selecionaTudo, this.ConnectionString);
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Video> videos = new List<Video>();

                while (sdr.Read())
                {
                    Video video = new Video();
                    video.Id = sdr.GetInt32(sdr.GetOrdinal("id"));
                    video.Titulo = sdr.IsDBNull(sdr.GetOrdinal("titulo")) ? "" : sdr.GetString(sdr.GetOrdinal("titulo"));
                    video.Descricao = sdr.IsDBNull(sdr.GetOrdinal("descricao")) ? "" : sdr.GetString(sdr.GetOrdinal("descricao"));
                    video.Url = sdr.IsDBNull(sdr.GetOrdinal("url")) ? "" : sdr.GetString(sdr.GetOrdinal("url"));

                    videos.Add(video);
                }

                return videos;
            }

        }
        public Video Get(int id)
        {
            Video video = new Video();
            using (ConnectionString)
            {
                ConnectionString.Open();
                string selecionaTudo = string.Format("SELECT * FROM VIDEO WHERE id={0}", id);

                SqlCommand cmd = new SqlCommand(selecionaTudo, this.ConnectionString);
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Video> videos = new List<Video>();

                if (sdr.Read())
                {
                    video.Id = sdr.GetInt32(sdr.GetOrdinal("id"));
                    video.Titulo = sdr.IsDBNull(sdr.GetOrdinal("titulo")) ? "" : sdr.GetString(sdr.GetOrdinal("titulo"));
                    video.Descricao = sdr.IsDBNull(sdr.GetOrdinal("descricao")) ? "" : sdr.GetString(sdr.GetOrdinal("descricao"));
                    video.Url = sdr.IsDBNull(sdr.GetOrdinal("url")) ? "" : sdr.GetString(sdr.GetOrdinal("url"));
                }
            }

            return video;
        }

        [ResponseType(typeof(Video))]
        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody] Video video)
        {
            if (string.IsNullOrEmpty(video.Descricao) || string.IsNullOrEmpty(video.Titulo) || string.IsNullOrEmpty(video.Url))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                using (ConnectionString)
                {
                    ConnectionString.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO VIDEO(titulo,descricao,url) output INSERTED.ID " +
                        "VALUES( @titulo, @descricao, @url)", ConnectionString))
                    {
                        cmd.Parameters.AddWithValue("@titulo", video.Titulo);
                        cmd.Parameters.AddWithValue("@descricao", video.Descricao);
                        cmd.Parameters.AddWithValue("@url", video.Url);

                        video.Id = (int)cmd.ExecuteScalar();
                    }
                }
            } catch (Exception)
            {
                return request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return request.CreateResponse(HttpStatusCode.OK, video);
        }

        public void Put(int id, [FromBody] string value)
        {
        }

        public void Delete(int id)
        {
        }
    }
}