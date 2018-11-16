using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace VSTSWorkItemShortcut
{
    public partial class formhelp : Form
    {
        private string fileName = string.Empty;

        private bool ctrlPressed = false;

        public formhelp()
        {
            InitializeComponent();
        }

        public formhelp(string fname)
        {
            InitializeComponent();
            fileName = fname;
        }

        private void formhelp_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.fileName))
            {
                if (!File.Exists(this.fileName))
                    this.Close();

                this.textBox1.AppendText(File.ReadAllText(this.fileName));
                this
                   .Writestatus($"Font info : {this.textBox1.Font.FontFamily.Name}, {this.textBox1.Font.Size}. Use Ctrl+MouseWheel to increase font size")
                   .ConfigureAwait(false);
            }
            else { this.Close(); }

            Hook.AppEvents().KeyDown += async (s2, e2) =>
                                        {
                                            if (e2.Control)
                                            {
                                                ctrlPressed = true;
                                                //writestatus($"ctrl pressed, Font Size: {this.textBox1.Font.Size}");
                                            }
                                        };
            Hook.AppEvents().KeyUp += async (s3, e3) =>
                                      {
                                          //writestatus($"KeyUp: {e3.KeyData.ToString()}");
                                          if (e3.KeyData == Keys.ControlKey)
                                          {
                                              ctrlPressed = false;
                                              //writestatus($"ctrl released, Font Size: {this.textBox1.Font.Size}");
                                          }
                                      };

            Hook.AppEvents().MouseWheel += async (s1, e1) =>
                                           {
                                               if (e1.Delta > 0 && ctrlPressed)
                                               {
                                                   if (this.textBox1.Font.Size.Between(8, 39))
                                                       this.textBox1.Font =
                                                           new Font(this.textBox1.Font.FontFamily,
                                                                    this.textBox1.Font.Size + 1);

                                                   await
                                                       Writestatus($"Font info : {this.textBox1.Font.FontFamily.Name}, {this.textBox1.Font.Size}. Use Ctrl+MouseWheel to increase font size")
                                                          .ConfigureAwait(false);
                                               }
                                               else if (e1.Delta < 0 && ctrlPressed)
                                               {
                                                   if (this.textBox1.Font.Size.Between(9, 40))
                                                   {
                                                       this.textBox1.Font =
                                                           new Font(this.textBox1.Font.FontFamily,
                                                                    this.textBox1.Font.Size - 1);
                                                   }

                                                   await
                                                       Writestatus($"Font info : {this.textBox1.Font.FontFamily.Name}, {this.textBox1.Font.Size}. Use Ctrl+MouseWheel to increase font size")
                                                          .ConfigureAwait(false);
                                               }
                                           };
        }

        protected async virtual Task Writestatus(string msg)
        {
            this.lblstatus.Text = msg;
        }
    }
}