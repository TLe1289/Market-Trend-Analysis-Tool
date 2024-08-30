namespace CorrectedCandlesticks
{
    partial class Form_Candlesticks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button_load = new System.Windows.Forms.Button();
            this.openFileDialog_Read = new System.Windows.Forms.OpenFileDialog();
            this.button_update = new System.Windows.Forms.Button();
            this.dateTimePicker_start = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_end = new System.Windows.Forms.DateTimePicker();
            this.chart_Candlesticks = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_end = new System.Windows.Forms.Label();
            this.label_start = new System.Windows.Forms.Label();
            this.comboBox_CandleType = new System.Windows.Forms.ComboBox();
            this.candlestickClassBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candlesticks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.candlestickClassBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button_load
            // 
            this.button_load.Location = new System.Drawing.Point(312, 383);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(75, 23);
            this.button_load.TabIndex = 0;
            this.button_load.Text = "load";
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Click += new System.EventHandler(this.button_load_Click);
            // 
            // openFileDialog_Read
            // 
            this.openFileDialog_Read.FileName = "openFileDialog1";
            this.openFileDialog_Read.Multiselect = true;
            this.openFileDialog_Read.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_Read_FileOk);
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(407, 383);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(75, 23);
            this.button_update.TabIndex = 1;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // dateTimePicker_start
            // 
            this.dateTimePicker_start.Location = new System.Drawing.Point(101, 418);
            this.dateTimePicker_start.Name = "dateTimePicker_start";
            this.dateTimePicker_start.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker_start.TabIndex = 2;
            this.dateTimePicker_start.Value = new System.DateTime(2022, 1, 1, 0, 0, 0, 0);
            // 
            // dateTimePicker_end
            // 
            this.dateTimePicker_end.Location = new System.Drawing.Point(495, 418);
            this.dateTimePicker_end.Name = "dateTimePicker_end";
            this.dateTimePicker_end.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker_end.TabIndex = 3;
            this.dateTimePicker_end.Value = new System.DateTime(2022, 3, 31, 0, 0, 0, 0);
            // 
            // chart_Candlesticks
            // 
            chartArea1.AlignWithChartArea = "ChartArea_Volume";
            chartArea1.Name = "ChartArea_OHLC";
            chartArea2.AlignWithChartArea = "ChartArea_OHLC";
            chartArea2.Name = "ChartArea_Volume";
            this.chart_Candlesticks.ChartAreas.Add(chartArea1);
            this.chart_Candlesticks.ChartAreas.Add(chartArea2);
            this.chart_Candlesticks.Location = new System.Drawing.Point(-2, 3);
            this.chart_Candlesticks.Name = "chart_Candlesticks";
            series1.ChartArea = "ChartArea_OHLC";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PriceDownColor=Red, PriceUpColor=Lime";
            series1.IsXValueIndexed = true;
            series1.Name = "Series_OHLC";
            series1.XValueMember = "Date";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValueMembers = "High,Low,Open,Close";
            series1.YValuesPerPoint = 4;
            series2.ChartArea = "ChartArea_Volume";
            series2.IsXValueIndexed = true;
            series2.Name = "Series_Volume";
            series2.XValueMember = "Date";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series2.YValueMembers = "Volume";
            this.chart_Candlesticks.Series.Add(series1);
            this.chart_Candlesticks.Series.Add(series2);
            this.chart_Candlesticks.Size = new System.Drawing.Size(802, 374);
            this.chart_Candlesticks.TabIndex = 5;
            this.chart_Candlesticks.Text = "chart1";
            // 
            // label_end
            // 
            this.label_end.AutoSize = true;
            this.label_end.Location = new System.Drawing.Point(560, 402);
            this.label_end.Name = "label_end";
            this.label_end.Size = new System.Drawing.Size(52, 13);
            this.label_end.TabIndex = 6;
            this.label_end.Text = "End Date";
            // 
            // label_start
            // 
            this.label_start.AutoSize = true;
            this.label_start.Location = new System.Drawing.Point(182, 402);
            this.label_start.Name = "label_start";
            this.label_start.Size = new System.Drawing.Size(55, 13);
            this.label_start.TabIndex = 7;
            this.label_start.Text = "Start Date";
            // 
            // comboBox_CandleType
            // 
            this.comboBox_CandleType.FormattingEnabled = true;
            this.comboBox_CandleType.Location = new System.Drawing.Point(341, 412);
            this.comboBox_CandleType.Name = "comboBox_CandleType";
            this.comboBox_CandleType.Size = new System.Drawing.Size(121, 21);
            this.comboBox_CandleType.TabIndex = 8;
            this.comboBox_CandleType.SelectedIndexChanged += new System.EventHandler(this.comboBox_CandleType_SelectedIndexChanged);
            // 
            // candlestickClassBindingSource
            // 
            this.candlestickClassBindingSource.DataSource = typeof(CorrectedCandlesticks.Candlestick_Class);
            // 
            // Form_Candlesticks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBox_CandleType);
            this.Controls.Add(this.label_start);
            this.Controls.Add(this.label_end);
            this.Controls.Add(this.chart_Candlesticks);
            this.Controls.Add(this.dateTimePicker_end);
            this.Controls.Add(this.dateTimePicker_start);
            this.Controls.Add(this.button_update);
            this.Controls.Add(this.button_load);
            this.Name = "Form_Candlesticks";
            this.Text = "Candlestick display";
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candlesticks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.candlestickClassBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_load;
        private System.Windows.Forms.OpenFileDialog openFileDialog_Read;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.DateTimePicker dateTimePicker_start;
        private System.Windows.Forms.DateTimePicker dateTimePicker_end;
        private System.Windows.Forms.BindingSource candlestickClassBindingSource;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Candlesticks;
        private System.Windows.Forms.Label label_end;
        private System.Windows.Forms.Label label_start;
        private System.Windows.Forms.ComboBox comboBox_CandleType;
    }
}

