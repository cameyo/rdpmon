using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Cameyo.RdpMon
{
    public partial class ShadowForm : Form
    {
        public long[] wtsSessionIds;

        public ShadowForm()
        {
            InitializeComponent();
        }

        private void ShadowForm_Load(object sender, EventArgs e)
        {
            var shadowVal = Utils.RegReadDword(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services", "Shadow");
            elevationImg.Visible = (!Utils.IsElevated() && shadowVal != 4);
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            var shadowVal = Utils.RegReadDword(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services", "Shadow");
            int exitCode = 0;
            if (noConsentRadio.Checked)
            {
                if (shadowVal != 4)
                {
                    if (Utils.IsElevated())
                        Utils.RegWriteDword(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services", "Shadow", 4);
                    else
                        Utils.ExecProg(Utils.MyExe(), "-ShadowNoConsent", ref exitCode, int.MaxValue, true);
                }
            }

            foreach (var wtsSessionId in wtsSessionIds)
            {
                var args = "/shadow:" + wtsSessionId.ToString();
                if (noConsentRadio.Checked)
                    args += " /noConsentPrompt";
                Utils.ExecProg("mstsc.exe", args, ref exitCode, 0, false);
            }
            DialogResult = DialogResult.OK;
        }
    }
}
