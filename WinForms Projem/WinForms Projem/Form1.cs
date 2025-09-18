using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text;
using System.IO;

namespace WinForms_Projem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            borderlessNumericUpDown1.Text = "";
        }

        static string url = "http://localhost:49943/";
        static List<User> users = new List<User>();
        static bool kullaniciYok = false;

        static ListView lv = new ListView();
        static int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;

        private async Task GetUsers(bool a)
        {
            string tempUrl = url;
            if (a) tempUrl += "a";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage cevap = await client.GetAsync(tempUrl);
                    cevap.EnsureSuccessStatusCode();

                    string JSON = await cevap.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(JSON);
                    JsonElement root = doc.RootElement;
                    int c = root.GetArrayLength();

                    if (c == 0)
                    {
                        kullaniciYok = true;
                        MessageBox.Show("Kullanıcı yok. Kullanıcı ekleyebilirsiniz!");
                        return;
                    }

                    var data = JsonSerializer.Deserialize<List<User>>(JSON);

                    for (int i = 0; i < c; i++)
                        users.Add(new User { isim = data[i].isim, yas = data[i].yas });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async Task AddUser()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string tempUrl = url + "add";

                    string JSON = JsonSerializer.Serialize<User>(userToAdd);
                    var icerik = new StringContent(JSON, Encoding.UTF8, "application/json");
                    HttpResponseMessage cevap = await client.PostAsync(tempUrl, icerik);
                    cevap.EnsureSuccessStatusCode();

                    textBox1.Text = "";
                    borderlessNumericUpDown1.Value = 0;
                    borderlessNumericUpDown1.Text = "";

                    userToAdd = new User();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static int IDToLookUp = 1;
        private async Task GetByID()
        {
            string tempUrl = url + IDToLookUp.ToString();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage cevap = await client.GetAsync(tempUrl);
                    cevap.EnsureSuccessStatusCode();
                    string JSON = await cevap.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<User>(JSON);

                    users.Add(new User { isim = data.isim, yas = data.yas });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static bool HasVerticalScrollBar(ListView LV)
        {
            int visibleItems = LV.ClientSize.Height / LV.GetItemRect(0).Height;
            return LV.Items.Count > visibleItems;
        }

        void createLV(List<User> users)
        {
            groupBox1.Controls.Remove(lv);
            lv.Dispose();

            lv = new ListView();
            lv.View = View.Details;

            lv.TabStop = false;

            lv.Left = button1.Left;
            lv.Top = button4.Bottom + 10;

            lv.Width = button1.Width;
            lv.Height = this.ClientSize.Height - lv.Top - 20;

            Font currentFont = lv.Font;

            lv.Font = new Font(currentFont.FontFamily, 20);

            ColumnHeader gorunmez = new ColumnHeader();
            gorunmez.Width = 0;
            gorunmez.Text = "";
            lv.Columns.Add(gorunmez);

            foreach (User user in users)
                lv.Items.Add(new ListViewItem(new[] { "", user.isim, user.yas.ToString() }));

            ColumnHeader isim = new ColumnHeader();
            isim.TextAlign = HorizontalAlignment.Center;
            isim.Width = HasVerticalScrollBar(lv) ? ((lv.ClientSize.Width - verticalScrollBarWidth) / 2) : (lv.ClientSize.Width / 2);
            isim.Text = "İsim";

            ColumnHeader yas = new ColumnHeader();
            yas.TextAlign = HorizontalAlignment.Center;
            yas.Width = HasVerticalScrollBar(lv) ? ((lv.ClientSize.Width - verticalScrollBarWidth) / 2) : (lv.ClientSize.Width / 2);
            yas.Text = "Yaş";

            lv.Columns.Add(isim);
            lv.Columns.Add(yas);

            groupBox1.Controls.Add(lv);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            users = new List<User>();

            await GetUsers(false);

            if (kullaniciYok)
            {
                kullaniciYok = false;
                return;
            }

            createLV(users);
        }

        User userToAdd = new User();
        private async void button2_Click(object sender, EventArgs e)
        {
            if (borderlessNumericUpDown1.Value < 0 || string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox1.Text[0].ToString()) || string.IsNullOrEmpty(borderlessNumericUpDown1.Text))
            {
                MessageBox.Show("Lütfen \"İsim\" kısmına bir metin, \"Yaş\" kısmına bir sayı girdiğinizden emin olun.");
            }

            else
            {
                userToAdd.isim = textBox1.Text;
                userToAdd.yas = (int)borderlessNumericUpDown1.Value;

                await AddUser();

                button1_Click(button1, e);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            IDToLookUp = (int)borderlessNumericUpDown2.Value;

            users = new List<User>();
            await GetByID();

            createLV(users);

            IDToLookUp = 1;
            borderlessNumericUpDown2.Value = 1;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            users = new List<User>();

            await GetUsers(true);

            if (kullaniciYok)
            {
                kullaniciYok = false;
                return;
            }

            createLV(users);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button2_Click(button2, e);
        }

        private void borderlessNumericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button2_Click(button2, e);
        }

        private void borderlessNumericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button2_Click(button2, e);
        }
    }

    public class User
    {
        public string isim { get; set; }
        public int yas { get; set; }
    }

    public class BorderlessNumericUpDown : NumericUpDown
    {
        public void UpButton()
        {
            base.UpButton();
        }

        public void DownButton()
        {
            base.DownButton();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (Controls.Count > 0 && Controls[0] is UpDownBase upDown)
            {
            }

            if (Controls.Count > 0)
            {
                Controls[0].Visible = false;
            }
        }
    }
}
