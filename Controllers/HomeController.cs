using Engelsiz_Rehber_Admin_Paneli.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Engelsiz_Rehber_Admin_Paneli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        string connectionString = "server=sql.freedb.tech;database=freedb_EngelsizRehberDB;uid=freedb_AdminER;password=sJ!Qs85*EQ!9d2*";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult DeleteEvent(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Event WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }

                ViewBag.Message = "Etkinlik başarıyla silindi.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


        public IActionResult Messaging()
        {
            return View();
        }
        public IActionResult Index()
        {
            List<Event> events = new List<Event>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Event ORDER BY start_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Event ev = new Event();
                                ev.id = Convert.ToInt32(reader["id"]);
                                ev.title = reader["title"].ToString();
                                ev.content = reader["content"].ToString();
                                ev.startDate = reader["start_date"].ToString();
                                ev.finishDate = reader["finish_date"].ToString();
                                ev.eventLocal = reader["event_Local"].ToString();
                                events.Add(ev);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Hata: " + ex.Message;
            }

            return View("Index", events);
        }

        public IActionResult Users()
        {

            return View();

        }

        public IActionResult AddEvent()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult AddEvent(Event model)
        {
            // Veritabanı bağlantısı oluşturma
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                // Bağlantıyı açma
                connection.Open();

                // SQL sorgusu oluşturma
                string query = "INSERT INTO Event (id, title, content, start_date, finish_date, event_Local) VALUES(NULL, @title, @content, @start_date, @finish_date, @event_Local)";

                // SQL komutunu oluşturma
                MySqlCommand command = new MySqlCommand(query, connection);

                // Parametreleri ekleyerek SQL komutunu güvenli hale getirme
                command.Parameters.AddWithValue("@title", model.title);
                command.Parameters.AddWithValue("@content", model.content);
                command.Parameters.AddWithValue("@start_date", model.startDate);
                command.Parameters.AddWithValue("@finish_date", model.finishDate);
                command.Parameters.AddWithValue("@event_Local", model.eventLocal);

                // SQL komutunu çalıştırma
                command.ExecuteNonQuery();

                ViewBag.Message = "Giriş başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Hata: " + ex.Message;
            }
            finally
            {
                // Bağlantıyı kapatma
                connection.Close();
            }

            return RedirectToAction("Index");
        }

    }
}
