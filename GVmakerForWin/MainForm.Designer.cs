﻿namespace GVmakerForWin
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.mainCanvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // mainCanvas
            // 
            this.mainCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainCanvas.Location = new System.Drawing.Point(0, 0);
            this.mainCanvas.Name = "mainCanvas";
            this.mainCanvas.Size = new System.Drawing.Size(688, 433);
            this.mainCanvas.TabIndex = 0;
            this.mainCanvas.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 433);
            this.Controls.Add(this.mainCanvas);
            this.Name = "MainForm";
            this.Text = "文曲星GVmaker模拟器";
            ((System.ComponentModel.ISupportInitialize)(this.mainCanvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mainCanvas;
    }
}

