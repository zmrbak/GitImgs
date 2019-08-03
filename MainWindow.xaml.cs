using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace GitImgs
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetUrl();
        }

        private void GetUrl()
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            String strhtml = webClient.DownloadString(this.gitUrl.Text);


            Regex reg = new Regex(@"(?is)<a[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?>");
            MatchCollection match = reg.Matches(strhtml);
            StringBuilder stringBuilder = new StringBuilder();

            List<string> urlList = new List<string>();
            foreach (Match m in match)
            {
                String href = m.Groups["href"].Value;
                if (System.IO.Path.GetExtension(href) == ".png" || System.IO.Path.GetExtension(href) == ".jpg")
                {
                    String fileName = System.IO.Path.GetFileName(href);

                    Boolean isFound = false;
                    foreach (var img in urlList)
                    {
                        if (fileName == img)
                        {
                            isFound = true;
                            break;
                        }
                    }

                    if (isFound == false)
                    {
                        urlList.Add(fileName);
                    }
                }
            }

            //乱序
            Shuffle(ref urlList);

            String imageUrl = this.gitUrl.Text;
            imageUrl = imageUrl.Replace("https://github.com", "https://raw.githubusercontent.com");
            imageUrl= imageUrl.Replace("tree/master/", "master/");
            imageUrl = imageUrl.Trim('/');

            foreach (var img in urlList)
            {
                String href = @"<li><img src="""+ imageUrl + "/" + img + @"""><br/>";

                stringBuilder.Append(href).AppendLine();
            }

            this.imagText.Text = stringBuilder.ToString();
        }

        public void Shuffle<T>(ref List<T> list)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            //儲存結果的集合
            List<T> newList = new List<T>();
            foreach (T item in list)
            {
                newList.Insert(rand.Next(0, newList.Count), item);
            }
            //移除list[0]的值
            newList.Remove(list[0]);

            //再重新隨機插入第一筆
            newList.Insert(rand.Next(0, newList.Count), list[0]);
            list = newList;
        }
    }
}

