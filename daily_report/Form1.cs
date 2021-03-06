﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace daily_report
{
    public partial class Form1 : Form
    {
        today_report today;

        public Form1()
        {
            InitializeComponent();
            this.today = new today_report();

            if (Settings1.Default.output_dir_path == "")
            {
                MessageBox.Show("日報の出力先が設定されていないので，設定をお願いします");
                this.set_output_dir_path();
            }


        }
        private void button1_Click(object sender, EventArgs e)//日報のトピックを登録するボタンが押されたとき
        {
            if (this.textBox1.Text == "")//一応入力のチェックはする
            {
                MessageBox.Show("なんもしてねえのかよ！");
            }
            else
            {
                this.today.add_content(this.textBox1.Text);
            }

            this.textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)//出力ボタンが押されたとき
        {
            StreamWriter w = new StreamWriter(this.get_output_dir_path() + @"\" +DateTime.Now.ToString("yyyymmdd") + "_daily_report.txt");
            this.today.report(w);
            w.Close();
            MessageBox.Show("出力しました！");
            System.Diagnostics.Process.Start(this.get_output_dir_path());
        }

        private void button3_Click(object sender, EventArgs e)//出力先変更ボタンが押されたとき
        {
            this.set_output_dir_path();
        }

        private void set_output_dir_path()//出力先を設定する．
        {
            /*デバッグ用出力先
            MessageBox.Show(Properties.Settings.Default.output_dir_path);
            MessageBox.Show(System.Environment.GetEnvironmentVariable("HOME"));
            */

            // ダイアログの説明を設定する
            folderBrowserDialog1.Description = "出力先フォルダを選んでください";

            // ルートになる特殊フォルダを設定する (初期値 SpecialFolder.Desktop)
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyDocuments;

            // 初期選択するパスを設定する
            folderBrowserDialog1.SelectedPath = System.Environment.GetEnvironmentVariable("HOME");

            // [新しいフォルダ] ボタンを表示する (初期値 true)
            folderBrowserDialog1.ShowNewFolderButton = true;

            // ダイアログを表示し、戻り値が [OK] の場合は、選択したディレクトリを表示する
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(folderBrowserDialog1.SelectedPath);
                Settings1.Default.output_dir_path = folderBrowserDialog1.SelectedPath;
            }
            else
            {
                MessageBox.Show("問題があったため，初期設定を利用します");
            }
            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            folderBrowserDialog1.Dispose();
            Settings1.Default.Save();
        }

        private string get_output_dir_path()
        {
            return Settings1.Default.output_dir_path;
        }
    }

    //今日なにしたかをまとめておくクラス
    public class today_report
    {
        what_i_did[] contents;

        public today_report()
        {
            this.contents = new what_i_did[0];
        }

        public void add_content(String contents)
        {
            int length = this.contents.Length;
            Array.Resize(ref this.contents, length+1);
            this.contents[length] = new what_i_did(contents);

        }

        public void report(StreamWriter writer)
        {
            foreach(what_i_did content in this.contents)
            {
                writer.WriteLine( content.show_what_did());
            }
        }
    }

    //この時間に何をしたか，時間とした内容をまとめておくクラス
    public class what_i_did
    {
        String content;
        DateTime dt;

        public what_i_did(String input_content)
        {
            this.content = input_content;
            dt = DateTime.Now;
        }

        public String show_what_did()
        {
            return this.dt.ToString("yyyy年MM月dd日 HH時mm分ss秒 :: ") + this.content;// +"\n";
        }
    }
}
