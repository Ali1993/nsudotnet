using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Xml;
using System.IO;
using System.Net;
using System.Net.Mail;
namespace RSSReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Объект класса рисунка
        ImageOfChanel imageChanel = new ImageOfChanel(); 

        //Массив элементов item
        Items[] articles;

        //Объект класса ChannelClass
        ChannelClass channel = new ChannelClass(); 

        private void button1_Click(object sender, EventArgs e)
        {
            //Запуск двух методов:

            //1. getNewMessage - получение списка сообщений по
            //указанному URL RSS ленты.

            //2. generateHtml - генерация html страницы 
            //для просмотра сообщений в webBrowser1.
            if (getNewMessage(@textBox1.Text) == true && generateHtml() == true)
            {
                //Открытие в  webBrowser1, сформированной страницы.
                webBrowser1.Navigate(Environment.CurrentDirectory + "\\Message.html");                
            }
        }

        //Метод принимает в качестве параметра ссылку на RSS-поток,
        //и возвращает либо true при успешном выполнении,
        //либо false при неудачной попытке.        
        bool getNewMessage(string url)
        {
            //Весь код помещен в try...catch
            //для отслеживания  исключений
            // и вывода их в виде сообщения.
            try
            {
                //Для предотвращения ошибки 407 (Удаленный сервер возвратил ошибку: 
                //(407) Требуется проверка подлинности посредника)
                //в сетях с прокси сервером 
                //загрузка RSS ленты осуществляется через класс WebRequest
                //с указанием настроек прокси.
                WebRequest wr = WebRequest.Create(url);

                //Указываем системные учетные данные приложения,
                //передаваемые прокси-серверу для выполнения проверки подлинности.
                wr.Proxy.Credentials = CredentialCache.DefaultCredentials;

                //Инициализируем класс XmlTextReader, который
                //обеспечивает прямой доступ (только для чтения) к потокам данных XML.
                //и передаем ему экземпляр класса System.IO.Stream(GetResponseStream)
                //для чтения данных из интернет-ресурса.
                XmlTextReader xtr = new XmlTextReader(wr.GetResponse().GetResponseStream());

                //Инициализируем класс "XmlDocument". Который представляет XML-документ
                //и включает в себя метод "Load",
                //предназначенный для загрузки документа 
                //с помощью объекта "XMLReader". 
                XmlDocument doc = new XmlDocument();
                doc.Load(xtr);

                //XmlNode root - содержит корневой элемент XML для 
                //загруженного элемента.
                XmlNode root = doc.DocumentElement;

                //Получаем количесво элементов item в RSS-потоке,
                //используя SelectNodes() и
                //выражение XPath, которое позволяет это сделать.
                articles = new Items[root.SelectNodes("//rss/channel/item").Count];

                //Инициализируем класс System.Xml.XmlNodeList, 
                //содержащий все дочерние узлы данного потока (channel).
                XmlNodeList nodeList;
                nodeList = root.ChildNodes;

                //Индикатор числового типа,
                //для массива articles[].
                int count = 0;

                //Цикл для прохода по всем каналам в RSS-потоке.
                foreach (XmlNode chanel in nodeList)
                {
                    //Цикл для прохода по всем элементам cnannel.                    
                    foreach (XmlNode chanel_item in chanel)
                    {
                        //Название канала RSS-потока.
                        if (chanel_item.Name == "title")
                        {
                            channel.title = chanel_item.InnerText;
                        }
                        //Описание канала RSS-потока.
                        if (chanel_item.Name == "description")
                        {
                            channel.description = chanel_item.InnerText;
                        }
                        //----
                        if (chanel_item.Name == "copyright")
                        {
                            channel.copyright = chanel_item.InnerText;
                        }
                        //Ссылка на сайт RSS-потока.
                        if (chanel_item.Name == "link")
                        {
                            channel.link = chanel_item.InnerText;
                        }
                        //Получение изображения канала RSS-потока.
                        if (chanel_item.Name == "img")
                        {
                            XmlNodeList imgList = chanel_item.ChildNodes;
                            foreach (XmlNode img_item in imgList)
                            {
                                if (img_item.Name == "url")
                                {
                                    imageChanel.imgURL = img_item.InnerText;
                                }
                                if (img_item.Name == "link")
                                {
                                    imageChanel.imgLink = img_item.InnerText;
                                }
                                if (img_item.Name == "title")
                                {
                                    imageChanel.imgTitle = img_item.InnerText;
                                }
                            }
                        }
                        //Обработка сообщения канала RSS-потока.
                        if (chanel_item.Name == "item")
                        {
                            XmlNodeList itemsList = chanel_item.ChildNodes;
                            articles[count] = new Items();

                            foreach (XmlNode item in itemsList)
                            {
                                //Заголовок сообщения.
                                if (item.Name == "title")
                                {
                                    articles[count].title = item.InnerText;
                                }
                                //Ссылка на сообщение в интернете.
                                if (item.Name == "link")
                                {
                                    articles[count].link = item.InnerText;
                                }
                                //Описание сообщения, по сути оно и является
                                //самим сообщением в формате HTML.
                                if (item.Name == "description")
                                {
                                    articles[count].description = item.InnerText;
                                }
                                //Дата публикации сообщения.
                                if (item.Name == "pubDate")
                                {
                                    articles[count].pubDate = item.InnerText;
                                }
                            }
                            //Увеличение счетчика сообщений
                            //для массива articles.
                            count += 1;
                        }
                    }
                }
                //После выполнения этого метода, 
                //обьекты классов, будут заполнены данными. 
                //В imageChanel содержатся все данные о рисунке (если он есть),
                //В channel - все параметры канала,
                //Массив articles - будет содержать все сообщения.  
                //И метод возвратит значение true.
                return true;
            }
            catch (Exception exc)
            {
                //Сообщение об ошибке при получении или распозновании данных.
                MessageBox.Show("Ошибка получения данных :" + exc.Message);

                //И метод возвратит значение false.
                return false;
            }
        }

        //Вывод полученных данных будет происходить
        //в элементе управления WebBrowser. Все данные из RSS-потока
        //будут сохранены в виде *.html файла, и последующей его загрузки.        
        bool generateHtml()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("Message.html"))
                {
                    //Начало формирования HTML страницы.
                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine(@"<meta http-equiv=""content-type"" content=""text/html; charset=utf-8"">");
                    
                    //Создание элемента <title> не является частью документа
                    //и не показывается напрямую на веб-странице. Данный элемент
                    //представляет собой, текст заголовка и
                    //отображается в левом верхнем углу окна браузера.                   
                    writer.WriteLine("<title>");
                    writer.WriteLine(channel.title);
                    writer.WriteLine("</title>");

                    //Стили применяемы к странице.
                    writer.WriteLine("<style type='text/css'>");
                    writer.WriteLine("A{color:#483D8B; text-decoration:none; font:Verdana;}");
                    writer.WriteLine("pre{font-family:courier;color:#000000;");
                    writer.WriteLine("background-color:#dfe2e5;padding-top:5pt;padding-left:5pt;");
                    writer.WriteLine("padding-bottom:5pt;border-top:1pt solid #87A5C3;");
                    writer.WriteLine("border-bottom:1pt solid #87A5C3;border-left:1pt solid #87A5C3;");
                    writer.WriteLine("border-right : 1pt solid #87A5C3;	text-align : left;}");
                    writer.WriteLine("</style>");
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");

                    //Вставка изображения из сообщения.
                    writer.WriteLine(@"<font size=""2"" face=""Verdana"">");
                    writer.WriteLine("<a href=\"" + imageChanel.imgLink + "\">");
                    writer.WriteLine("<img src=\"" + imageChanel.imgURL + "\" border=0></a>  ");

                    //Вывод заголовка(гиперссылки) RSS-потока - источника.
                    writer.WriteLine("<a href=\"" + channel.link + "\">" +
                                     @"<h2 align=""center"">" + channel.title + "</h2></a>");

                    //Вывод описания RSS-потока.
                    writer.WriteLine(@"<h3 align=""center"">" + channel.description + "</h3>");

                    //Формирование html таблицы с сообщениями.
                    writer.WriteLine(@"<table width=""80%"" align=""center"" border=1>");

                    //Переменная для подсчета количества и нумерации сообщений.
                    int count_element = 0;

                    //Очистка listBox1 перед каждой загрузкой 
                    //заголовков сообщений.
                    listBox1.Items.Clear();

                    //Вставка сообщений в таблицу.
                    foreach (Items article in articles)
                    {
                        //Формирование новой строки и ячейки таблицы.
                        writer.WriteLine("<tr>");
                        writer.WriteLine("<td>");

                        //Вставка  заголовка и даты создания сообщения.
                        writer.WriteLine("<br>  <a href=\"" + article.link + "\"><b>" + count_element + ". " + article.title + "</b></a>");
                        writer.WriteLine("& (" + article.pubDate + ")<br><br>");

                        //Вставка в ячейку подтаблицы с HTML кодом сообщения.
                        writer.WriteLine(@"<table width=""95%"" align=""center"" border=0>");
                        writer.WriteLine("<tr><td>");
                        writer.WriteLine(article.description);
                        writer.WriteLine("</td></tr></table>");

                        //Вставка ссылки на сообщение в интернете
                        writer.WriteLine("<br>  <a href=\"" + article.link + "\">");
                        writer.WriteLine(@"<font size=""2"" size=""right"" >Читать дальше >>> </font></a><br><br>");
                        writer.WriteLine("</td>");
                        writer.WriteLine("</tr>");

                        //Вставка в listBox1 заголовков сообщений.
                        listBox1.Items.Add(count_element+". "+article.title + " (" + article.pubDate + ")");
                        body += count_element + ". " + article.title + " (" + article.pubDate + ")";
                        //После вставки сообщения
                        //увеличиваем счеткик сообщений на 1.
                        count_element++;
                    }
                    //Закрывающий тег таблицы.
                    writer.WriteLine("</table><br>");

                    //Сообщение о правах на данную информацию,
                    //распологаемую по центру в конце страницы .
                    writer.WriteLine(@"<p align=""center"">");
                    writer.WriteLine("<a href=\"" + channel.link + "\">" + channel.copyright + "</a></p>");
                    writer.WriteLine("</font>");
                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");

                    //Вывод общего количества сообщений в label2.
                    label2.Text = "Всего сообщений: "+count_element.ToString();

                    //Если все выполнено успешно, метод возвратит true.
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Перехват ошибок и вывод в сообщении.
                MessageBox.Show(ex.Message);

                //Метод в данном случае вернет false.
                return false;
            }        
        }

        string body = null;

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("ТВОЙ ЭМЭИЛ");
                    mail.To.Add("ЭМЭИЛ ПОЛУЧАТЕЛЯ");
                    mail.Subject = "SSL";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    bool enableSSL = true;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);  //РАБОТАЕТ ДЛЯ ПОЧТЫ GMAIL

                    smtp.Credentials = new NetworkCredential("ТВОЙ ЭМЭИЛ", "ТВОЙ ПАРОЛЬ");
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);

                }
            }
            catch
            {

            }
        }
    }

    // Класс овечающий за настройки канала.
    public class ChannelClass
    {
        //Заголовок сайта - источника.
        public string title;

        //Описание сайта - источника.
        public string description;

        //Ссылка на сайт источника.
        public string link;

        //Права владельца сайта.
        public string copyright;

        public ChannelClass()
        {            
            title = "";
            description = "";
            link = "";
            copyright = "";
        }
    }

    // Класс рисунка канала.
    public class ImageOfChanel
    {
        //Заголовок изображения.
        public string imgTitle;

        //Ссылка на изображение.
        public string imgLink;

        //URL адрес сообщения.
        public string imgURL;

        public ImageOfChanel()
        {
            imgTitle = "";
            imgLink = "";
            imgURL = "";
        }
    }

    // Класс сообщений.
    public class Items
    {
        //Заголовок сообщения.
        public string title;
        //Ссылка на страницу сообщения в интернете.
        public string link;
        //Описание(текст сообщения).
        public string description;
        //Дата публикации сообщения.
        public string pubDate;

        public Items()
        {
            title = "";
            link = "";
            description = "";
            pubDate = "";
        }
    }
}
