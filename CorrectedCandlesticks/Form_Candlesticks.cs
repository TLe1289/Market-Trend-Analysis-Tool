using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace CorrectedCandlesticks
{
    public partial class Form_Candlesticks : Form
    {
        List<smartCandlestick_Class> Full_list = null;   //Holds the complete list of candlesticks so it can be filtered in the future
        List<smartCandlestick_Class> Filtered_list = null; //holds the list constricted by the start and end dates
        BindingList<Candlestick_Class> Binding_list = null; //bindin list is used to be implemented into the datagridview and chart. Comes from teh filtered_list (list)
        Dictionary<string, Recognizer> DOR = null;
        String FileName;//holds file name from the open file dialogue
        DateTime StartofIt;//date for datetimepicker_start
        DateTime EndofIt;//date for datetimepicker_end
        String SelectionType;//The string which will be used Candlestick type to filter the candlesticks and add annotations

        /// <summary>
        /// The first Candlestick constructor is used for the main form, which initilize candlesticks using the present file path
        /// and start and end dates. The second contructor uses paramters of the file path and the two dates. Necessary to create multiple
        /// of forms with different set of candlesticks with various different file paths.
        /// </summary>
        public Form_Candlesticks()
        {
            InitializeComponent(); 
        }
        public Form_Candlesticks(string path, DateTime StartDate, DateTime EndDate)
        {
            InitializeComponent();
            StartofIt = StartDate; //Initialize the start date to begin the filtering candlesticks  from filtering function
            EndofIt = EndDate;//Initialize the end date to end the filter of candlesticks from filtering funciton
            FileName = path; //path file is different for each form created. Changing FileName is necessary because its the main parameter for the read file function
        }

        /*button load reads file using the openfiledialogue and runs four functions to read the candlesticks from file, filter the candlesticks, and to normalize
         and display the chart. Primarily important because it sets the complete list so it can be filtered into the filtered list*/
        private void button_load_Click(object sender, EventArgs e)
        {
            openFileDialog_Read.Filter = "*.csv|";//filters the data based on the based on haveing .csv anything after "|" is will filter based on file select box
            if(openFileDialog_Read.ShowDialog() == DialogResult.OK)// "ShowDialog" displays the file system structure,"DialogResult.Ok" is the condition which the user selects a file
            { }
        }
        /*update primarily uses the complete list from load button to perform the three functions filtering, normalizing and displaying the chart*/
        private void button_update_Click(object sender, EventArgs e)
        {
            StartofIt = dateTimePicker_start.Value.Date;//Datetime variable recieve the start date from datetimepicker
            EndofIt = dateTimePicker_end.Value.Date;//Datetime vairable recieve the end date from a different datetimepicker
            filterCandlesticks(); //Calls on the void function to use 2 datetimes to filter the candlesticks in a new list of candlesticks within the date range
            normalizeChart(); // Calls on the void functions to used the filter list to calculate the min and max of the y axis for the candlestick chart
            displayChart();//input the filtered list into a BindingList and Use the binding list to display candlesticks on datagridview and chart
        }

        

        /// <summary>
        /// ReadCandlesticks take in the filename to extract a line of candles using StreamReader. The streamreader continues to go through each line until it reaches the end
        /// of the file. For each line it inputs the string into class constructor to create a candlestick object and adds it to the list. The function should return a complete list of candlesticks
        /// from the file
        /// </summary>
        private void readCandlesticksFromFile()
        {
            Full_list = readCandlesticksFromFile(FileName); //Calls the readCandlesticksFromFile() with parameter (filename) to extract the candlesticks from filename to place into a list --> Full_list
        }
        private List<smartCandlestick_Class> readCandlesticksFromFile(string filename)
        {
            const string referenceString = "Date,Open,High,Low,Close,Adj Close,Volume";//The Reference string which much correlate witht the first line because they are labels
            List<smartCandlestick_Class> listOfCandlesticks = new List<smartCandlestick_Class>(1024);//Initiate a tempoary empty candlestick list
            StreamReader stringstream = new StreamReader(filename); //Place the first line of filename into a Streamreader variable
            string line = stringstream.ReadLine(); // ReadLine() will transform Streamreader into string 
            if (line == referenceString)// An if statement to determin if the first line was the labels for the candlesticks (referenceString)
            {
                while (line != null)// As long as the line variable isn't empty it will continue to read through each line and create and insert candlesticks in a list
                {
                    line = stringstream.ReadLine();//The next Streamerreads should be data values which can be use to develop candle sticks so we place them into line
                    smartCandlestick_Class stick = new smartCandlestick_Class(line);// Calls on the Candlestick_Class constructor to develop candlesticks based on the data from "line"
                    listOfCandlesticks.Add(stick);// The candlestick has been made and will be added to the tempoarry list
                }
            }
            return listOfCandlesticks;//Point where the StreamerReader has no more data to read or end of file. It will then return the tempoary list and place it into Full_List
        }
        /// <summary>
        /// FilterCandlesticks take in the list of candlesticks we made from the read functions, and the start and end DateTimes that we gottenfrom the datetimepicker
        /// Once the filtered_candlestick list is cleared, we use a foreachloop to iterate through all the candlesticks of the unfiltered list. We then for each candlesticks datetime
        /// to determine if there after the start date and before the end date. If so we add to list. Once the loop end we return the filtered date list of candlesticks
        /// </summary>
        private void filterCandlesticks()
        {
            //DateTime StartofIt = dateTimePicker_start.Value.Date;//Datetime variable recieve the start date from datetimepicker
            //DateTime EndofIt = dateTimePicker_end.Value.Date;//Datetime vairable recieve the end date from a different datetimepicker
            Filtered_list = filterCandlesticks(Full_list, StartofIt, EndofIt);// calls on the filter function to used the full candlestick list and two date times to return a smaller list
        }
        List<smartCandlestick_Class> filterCandlesticks(List<smartCandlestick_Class> unfilteredList, DateTime startDate, DateTime endDate)
        {
            
            List<smartCandlestick_Class> SelectedlistOfCandlesticks = new List<smartCandlestick_Class>(1024);//Initiate a tempoary empty candlestick list
            if (Filtered_list != null)
            { //An if statement to determine if the list of candlesticks was not empty
                Filtered_list.Clear(); //the Filter_list was not empty and required being cleared before add a new filtered candlesticks
            }
            foreach (smartCandlestick_Class c in unfilteredList) //A foreach loop which iterates through all candlesticks within the complete list of candlesticks. Candestick "c" is iterator.
            {
                if (c.Date >= startDate && c.Date <= endDate)//The condition if the iterator candlestick is in range by being after the start date and before the end date.
                {
                    SelectedlistOfCandlesticks.Add(c); // By passing the condition of the two dates, the candlestick will be added into the tempoary list. 
                }
            }

            
            return SelectedlistOfCandlesticks;//This is the point which the for loop has gone through all candles of the fulllist. It will return the tempoary list so "Filtered_list" now holds all candles sticsk limited by date
        }

        /// <summary>
        /// Normalize chart takes in the filtered list of candlesticks that we created from the filter function and we also use the start and end datetimes from the datetimepicker. We use the same
        /// dates from filter function for consistency. Foreach loop will iterate through the entire list of candlesticks. There are if conditions to determine if the candlesticks high and low
        /// variable were greater or less than tempoary values, so we can save the highest and lowest, the objective is find variables highest and smallest variable to set as the axis for OHLC chart
        /// Because it sets the axis, it doesn't have a return type
        /// Normalize chart will also help add the specific candlestick elements into the combobox, by using the same for loop and checking each smartcandlestick identifies
        /// by a certain type using the true values in it's dictionary
        /// 
        /// Normalize Chart will also initialize a dictionary which holds all the recognizer classes which will help identify all
        /// the patterns within the all the candlesticks. It also initialize the combobox to hold all the necessary patterns
        /// </summary>
        private void normalizeChart()
        {
            normalizeChart(Filtered_list, StartofIt, EndofIt);// calls on the parameter functions to set min and max ranges of the candlestick chart axis
        }
        private void normalizeChart(List<smartCandlestick_Class> Filtered_list, DateTime startDate, DateTime endDate)
        {
            decimal Top_Y = decimal.MinValue;// A decimal variable to hold the lowest decimal but should be given the Highest decimal in the end
            decimal Bottom_Y = decimal.MaxValue;// A decimal varaible to hold the highest decmial but should decrement to be given the lowest decimal in the end
            foreach (Candlestick_Class candle in Filtered_list) //A foreach loop which iterate through the entire list of Filterd_list of candlesticks. iterator "candle"
            {
                if (candle.High > Top_Y)// The if conditions which the candlestick's High was greater than the current highest value
                {
                    Top_Y = candle.High;//if it was greater, will set the highest value to the candlesticks value
                }
                if (candle.Low < Bottom_Y)//The if condition which the candlestick's Low was smaller than the current lowest value
                {
                    Bottom_Y = candle.Low;//if it was less, will set the lowest value to the candlestick value
                } 


                /* //Initialize the candlestickbox based on if the smart candlestick is one of the categories
                smartCandlestick_Class TempoaryCandle = (smartCandlestick_Class) candle; //Create a smartcandlestick by casting from filtered list. Filtered candlesticks were previously given smartcandlesticks. Casting is necessary because Filterlist only sees candlessticks
                foreach (var measure in TempoaryCandle.DictionaryCandles)//Goes through each segment of dictionary to find if candlestick has a truth value to a certain candlestick type
                {
                    if (measure.Value)// only run if it found a value or candlestick type that is applicable for this candlestick
                    {
                        if (!comboBox_CandleType.Items.Contains(measure.Key))//only runs if the Combobox doesn't already contain this candlestick type
                        {
                            comboBox_CandleType.Items.Add(measure.Key);//Add candlestick type into the Combobox for this specific form
                        }
                    }
                }
                */
            }

            DOR = new Dictionary<string, Recognizer>();  //Dictionary which holds a pattern name with pattern recognizer
            Recognizer r; // r is an empty recognizer. It's from the abstract class, but not can't be directly access unless you use the subclasses
            r = new Recognizer_Bullish("Bullish", 1); //Empty recognizer is given Recognizer_Bullish class initilize with name and 1 size
            DOR.Add("Bullish", r); //add Bullish name and  recognizer into dictionary
            r = new Recognizer_Bearish("Bearish", 1);//Empty recognizer is given Recognizer_Bearish class initilize with name and 1 size
            DOR.Add("Bearish", r);//add Bearish name and  recognizer into dictionary
            r = new Recognizer_Neutral("Neutral", 1);//Empty recognizer is given Recognizer_Neutral class initilize with name and 1 size
            DOR.Add("Neutral", r); //add Neutral name and  recognizer into dictionary
            r = new Recognizer_Marubozu("Marubozu", 1);//Empty recognizer is given Recognizer_Marubozu class initilize with name and 1 size
            DOR.Add("Marubozu", r); //add Marubozu name and  recognizer into dictionary
            r = new Recognizer_Hammer("Hammer", 1);//Empty recognizer is given Recognizer_Hammer class initilize with name and 1 size
            DOR.Add("Hammer", r); //add Hammer name and  recognizer into dictionary
            r = new Recognizer_Doji("Doji", 1);//Empty recognizer is given Recognizer_Doji class initilize with name and 1 size
            DOR.Add("Doji", r); //add Doji name and  recognizer into dictionary
            r = new Recognizer_DragonflyDoji("DragonflyDoji", 1);//Empty recognizer is given Recognizer_DragonflyDoji class initilize with name and 1 size
            DOR.Add("DragonflyDoji", r); //add DragonflyDoji name and  recognizer into dictionary
            r = new Recognizer_GravestoneDoji("GravestoneDoji", 1);//Empty recognizer is given Recognizer_GravestoneDoji class initilize with name and 1 size
            DOR.Add("GravestoneDoji", r); //add GravestoneDoji name and  recognizer into dictionary
            r = new Recognizer_BullishEngulfing("BullishEngulfing", 2);//Empty recognizer is given Recognizer_BullishEngulfing class initilize with name and 2 size
            DOR.Add("BullishEngulfing", r); //add BullishEngulfing name and  recognizer into dictionary
            r = new Recognizer_BearishEngulfing("BearishEngulfing", 2);//Empty recognizer is given Recognizer_BearishEngulfing class initilize with name and 2 size
            DOR.Add("BearishEngulfing", r); //add BearishEngulfing name and  recognizer into dictionary
            r = new Recognizer_BullishHarami("BullishHarami", 2);//Empty recognizer is given Recognizer_BullishHarami class initilize with name and 2 size
            DOR.Add("BullishHarami", r); //add BullishHarami name and  recognizer into dictionary
            r = new Recognizer_BearishHarami("BearishHarami", 2);//Empty recognizer is given Recognizer_BearishEngulfing class initilize with name and 2 size
            DOR.Add("BearishHarami", r); //add BearishHarami name and  recognizer into dictionary
            r = new Recognizer_Peak("Peak", 3);//Empty recognizer is given Recognizer_Peak class initilize with name and 3 size
            DOR.Add("Peak", r); //add Peak name and  recognizer into dictionary
            r = new Recognizer_Valley("Valley",3);//Empty recognizer is given Recognizer_Valley class initilize with name and 3 size
            DOR.Add("Valley", r); //add Valley name and  recognizer into dictionary


            comboBox_CandleType.Items.Clear();//Empty the combo box each time we reset or update the form with new candlestick series
            DOR["Bullish"].recognizeALL(Filtered_list); //Runs Bullish class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Bullish"); //Insert combobox with pattern name Bullish
            DOR["Bearish"].recognizeALL(Filtered_list); //Runs Bearish class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Bearish");//Insert combobox with pattern name Bearish
            DOR["Neutral"].recognizeALL(Filtered_list); //Runs Neutral class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Neutral"); //Insert combobox with pattern name Neutral
            DOR["Marubozu"].recognizeALL(Filtered_list);//Runs Marubozu class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Marubozu"); //Insert combobox with pattern name Marubozu
            DOR["Hammer"].recognizeALL(Filtered_list); //Runs Hammer class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Hammer"); //Insert combobox with pattern name Hammer
            DOR["Doji"].recognizeALL(Filtered_list); //Runs Doji class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Doji"); //Insert combobox with pattern name Doji
            DOR["DragonflyDoji"].recognizeALL(Filtered_list); //Runs DragonflyDoji class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("DragonflyDoji"); //Insert combobox with pattern name DragonflyDoji
            DOR["GravestoneDoji"].recognizeALL(Filtered_list); //Runs GravestoneDoji class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("GravestoneDoji"); //Insert combobox with pattern name GravestoneDoji
            DOR["BullishEngulfing"].recognizeALL(Filtered_list); //Runs BullishEngulfing class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("BullishEngulfing"); //Insert combobox with pattern name BullishEngulfing
            DOR["BearishEngulfing"].recognizeALL(Filtered_list); //Runs BearishEngulfing class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("BearishEngulfing"); //Insert combobox with pattern name BearishEngulfing
            DOR["BullishHarami"].recognizeALL(Filtered_list); //Runs BullishHarami class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("BullishHarami"); //Insert combobox with pattern name BullishHarami
            DOR["BearishHarami"].recognizeALL(Filtered_list); //Runs BearishHarami class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("BearishHarami"); //Insert combobox with pattern name BearishHarami
            DOR["Peak"].recognizeALL(Filtered_list); //Runs Peak class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Peak"); //Insert combobox with pattern name Peak
            DOR["Valley"].recognizeALL(Filtered_list); //Runs Valley class's RecognizeALL from abstract class Recognizer to run recognize() on all candlesticks
            comboBox_CandleType.Items.Add("Valley"); //Insert combobox with pattern name Valley


            // the point which the for loop ends and Top_Y should hold the highest High and Bottom_Y should hold the lowest Low
            chart_Candlesticks.ChartAreas[0].AxisY.Minimum = (double)Bottom_Y * 0.98; //The minimum of chart's yaxis is set to lowest value subtracted by 2%. Also convert to decimal to match types
            chart_Candlesticks.ChartAreas[0].AxisY.Maximum = (double)Top_Y * 1.02;//The maximum of chart's yaxis is set to highest value added by 2%. Also convert to decimal to match types
        }
        /// <summary>
        /// Display chart will used the same filtered candlestick list created from the filter function to be set into the bindinglist. The bindinglist should enable the user to make direct
        /// changes to the list. We connnect the binding list with the datagridview and chart's datasource to display the candlesticks on the table and the two charts
        /// 
        /// </summary>
        private void displayChart()
        {
            displayChart(Filtered_list);// Calls on the DisplayChart() with parameter of a filtered list<Candlesticks> to display the chart and grid
        }
        private void displayChart(List<smartCandlestick_Class> Filtered_listOfCandlesticks)
        {
            List<Candlestick_Class> DisplayCandlestickList = new List<Candlestick_Class>(1024); //initialize with a CandlestickClass list
            foreach (smartCandlestick_Class scs  in Filtered_listOfCandlesticks) //There is a for loop which goes through all smartcandlesticks and add into the Candlestick class.
            {
                DisplayCandlestickList.Add(scs); //Output: the displaycandlestick class should holds candlesticks but hold smartcandlesticks
            }

            chart_Candlesticks.Annotations.Clear(); //When uploading a new form from openfildialogue, we clear all annotations
            Binding_list = new BindingList<Candlestick_Class>(DisplayCandlestickList);//Bindinglist is created using the filtered list as parameters, Make it so the list can be editable
            //dataGridView_Candles.DataSource = Binding_list;//DatagridView can display the data, by setting the "DataSource" to the bindinglist
            chart_Candlesticks.DataSource = Binding_list;//Setting the Chart's DataSource to link with the location of the binding list. 
            chart_Candlesticks.DataBind();//DataBind() will act as a push and will transfer the data from bindinglist to chart and display the candlesticks.
        }

        /// <summary>
        /// Smart Display Chart runs exactly the same way similar to DisplayChart function; however, it includes additional parameter of string Selectiontype
        /// SelectionType includes a string which identifies Candlestick type. This is necessary as the Chart will now include annotations to any smart candlesticks
        /// that associate with the same type
        /// </summary>
        private void smartDisplayChart()
        {
           smartDisplayChart(Filtered_list, SelectionType);//Calls on smartDisplayChart() with parameters list<candlestick> and string to display the chart and the annotations
        }
        private void smartDisplayChart(List<smartCandlestick_Class> Filtered_listOfCandlesticks, string StringChart)
        {
            List<Candlestick_Class> DisplayCandlestickList = new List<Candlestick_Class>(1024);//initialize with a CandlestickClass list
            foreach (smartCandlestick_Class scs in Filtered_listOfCandlesticks) //There is a for loop which goes through all smartcandlesticks and add into the Candlestick class.
            {
                DisplayCandlestickList.Add(scs); //Output: the displaycandlestick class should holds candlesticks but hold smartcandlesticks
            }
            chart_Candlesticks.Annotations.Clear(); //When uploading a new form from openfildialogue, we clear all annotations
            Binding_list = new BindingList<Candlestick_Class>(DisplayCandlestickList);//Bindinglist is created using the filtered list as parameters, Make it so the list can be editable
            //dataGridView_Candles.DataSource = Binding_list;//DatagridView can display the data, by setting the "DataSource" to the bindinglist
            chart_Candlesticks.DataSource = Binding_list;//Setting the Chart's DataSource to link with the location of the binding list. 
            chart_Candlesticks.DataBind();//DataBind() will act as a push and will transfer the data from bindinglist to chart and display the candlesticks.


            //Recognizer TempClass = DOR[StringChart];

            int number = DOR[StringChart].returnLength; //return the number candlesticks necessary for each pattern, it is based on the Patternlength variable from the abstract class

            
            foreach (DataPoint point in chart_Candlesticks.Series[0].Points)//the for loop goes through each element in the series. Series[0] reference the OHLC chart
            {
                double holdDouble = point.XValue;//records the date for the corresponding datapoint as a double
                DateTime holdDate = DateTime.FromOADate(holdDouble);//converts double into dateTime
                foreach (Candlestick_Class temp in Binding_list)//Iterate through each element of the binding list
                {
                    if (temp.Date == holdDate)//The objective of two iterations is to find the location within binding list which has the corresponding dateTime
                    {
                        smartCandlestick_Class smartTemp = (smartCandlestick_Class)temp;//Creates a smartcandlestick by tempoarility casting the temp candlestick
                        if (number == 1) //If the length is one it only needs one arrow to where the single patter is
                        {
                            if (smartTemp.DictionaryCandles[StringChart] == true)//Because it's now a smartcandlestick we can check within dictionary and within data type if it's true or corresponding to candlestick type
                            {
                                ArrowAnnotation arrowAnnotation = new ArrowAnnotation();//Create a new ArrowAnnotation
                                arrowAnnotation.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation.LineWidth = 2; //Arrow Design details
                                arrowAnnotation.AnchorDataPoint = point;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation.Width = 2; //Arrow Design details
                                arrowAnnotation.Height = 3; //Arrow Design details
                                arrowAnnotation.BackColor = Color.Black; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation);//The chart will now add this specific annotation into the chart
                            }
                            else//situation which the candlestick is not the same candlestick type
                            {
                                break; //will break from the for loop iterating through the Bindinglist and will continue the iteration through the Series[0]
                            }
                        }
                        else if (number == 2)// if the length is 2, it needs two arrow annotations, to points to two candlesticks for certain patter
                        {
                            if (smartTemp.DictionaryCandles[StringChart] == true) //If the candlestick return true for the two candlesticks pattern, go throught the condition
                            {
                                int Tempoaryindex = chart_Candlesticks.Series[0].Points.IndexOf(point); //Return the index of the current candlestick that had true
                                DataPoint nextPoint = null; //Initialize a next candlestick but set it to empty
                                if (Tempoaryindex < chart_Candlesticks.Series[0].Points.Count - 1) //If the current candlestick is at the far right or end do not go through this condition
                                {
                                    nextPoint = chart_Candlesticks.Series[0].Points[Tempoaryindex + 1];// Because the candlestick is not at the end, it's able to set a next point varaible as the right index from the point
                                }

                                /*RectangleAnnotation rectangleAnnotation = new RectangleAnnotation();
                                rectangleAnnotation.Height = 10;
                                rectangleAnnotation.Width = 5;
                                rectangleAnnotation.Text = "First";
                                rectangleAnnotation.LineWidth = 2;
                                rectangleAnnotation.BackColor = Color.Transparent;
                                rectangleAnnotation.AnchorDataPoint = point;
                                chart_Candlesticks.Annotations.Add(rectangleAnnotation);*/

                                ArrowAnnotation arrowAnnotation = new ArrowAnnotation();//Create a new ArrowAnnotation
                                arrowAnnotation.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation.LineWidth = 2; //Arrow Design details
                                arrowAnnotation.AnchorDataPoint = point;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation.Width = 2; //Arrow Design details
                                arrowAnnotation.Height = 3; //Arrow Design details
                                arrowAnnotation.BackColor = Color.Black; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation);

                                ArrowAnnotation arrowAnnotation2 = new ArrowAnnotation();//Create a new ArrowAnnotation
                                arrowAnnotation2.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation2.LineWidth = 2; //Arrow Design details
                                arrowAnnotation2.AnchorDataPoint = nextPoint;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation2.Width = 2; //Arrow Design details
                                arrowAnnotation2.Height = 3; //Arrow Design details
                                arrowAnnotation2.BackColor = Color.Aqua; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation2);
                            }
                            else//situation which the candlestick is not the same candlestick type
                            {
                                break; //will break from the for loop iterating through the Bindinglist and will continue the iteration through the Series[0]
                            }
                        }
                        else if (number == 3) //if the number is three it needs three arrow annotations, to represent three candlestick patterns
                        {
                            if (smartTemp.DictionaryCandles[StringChart] == true) //If the candlestick return true for the two candlesticks pattern, go throught the conditio
                            {
                                int Tempoaryindex = chart_Candlesticks.Series[0].Points.IndexOf(point);  //Return the index of the current candlestick that had true
                                DataPoint nextPoint = null; //Initialize the next and prev data points as empty.
                                DataPoint prevPoint = null;
                                if (Tempoaryindex < chart_Candlesticks.Series[0].Points.Count - 1 && Tempoaryindex > 0) //Make sure teh selected Candlesick is not he begginng or the end
                                {
                                    prevPoint = chart_Candlesticks.Series[0].Points[Tempoaryindex - 1]; //prevPoint is the candlestick before the point
                                    nextPoint = chart_Candlesticks.Series[0].Points[Tempoaryindex + 1]; //nextPoint is the candlestic after the point
                                }

                                ArrowAnnotation arrowAnnotation = new ArrowAnnotation();//Create a new ArrowAnnotation
                                arrowAnnotation.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation.LineWidth = 2; //Arrow Design details
                                arrowAnnotation.AnchorDataPoint = point;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation.Width = 2; //Arrow Design details
                                arrowAnnotation.Height = 3; //Arrow Design details
                                arrowAnnotation.BackColor = Color.Black; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation);

                                ArrowAnnotation arrowAnnotation2 = new ArrowAnnotation();//Create a new ArrowAnnotation
                                arrowAnnotation2.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation2.LineWidth = 2; //Arrow Design details
                                arrowAnnotation2.AnchorDataPoint = nextPoint;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation2.Width = 2; //Arrow Design details
                                arrowAnnotation2.Height = 3; //Arrow Design details
                                arrowAnnotation2.BackColor = Color.Aqua; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation2);

                                ArrowAnnotation arrowAnnotation3 = new ArrowAnnotation();
                                arrowAnnotation3.ArrowSize = 2; //Arrow Design details
                                arrowAnnotation3.LineWidth = 2; //Arrow Design details
                                arrowAnnotation3.AnchorDataPoint = prevPoint;//Set ArrowAnotations to bind with the corresponding datapoint within the Series of Chart OHLC. The location should point directly to corresponding candlestick
                                arrowAnnotation3.Width = 2; //Arrow Design details
                                arrowAnnotation3.Height = 3; //Arrow Design details
                                arrowAnnotation3.BackColor = Color.Aqua; //Arrow Design details
                                chart_Candlesticks.Annotations.Add(arrowAnnotation3);
                            }
                        }
                        else { }

                    }
                }
            }
            
            
            

      
        }
        /// <summary>
        /// A function which groups all the necessary functions needed for eahc form to read through candlesticks and filter the data
        /// This also include normalizing and databinding. 
        /// </summary>
        private void readandDisplayStock()
        {
            readCandlesticksFromFile();//Calls on the read functions to read from file path and transfer all candlestick data into a SmartCandlestick list 
            filterCandlesticks(); //Calls on the void function to use 2 datetimes to filter the candlesticks in a new list of candlesticks within the date range
            normalizeChart(); // Calls on the void functions to used the filter list to calculate the min and max of the y axis for the candlestick chart
            displayChart();//input the filtered list into a BindingList and Use the binding list to display candlesticks on datagridview and chart
        }//You are going to need this funciton in the future

        /*Function created as an eventhandler for the opening files using open file dialogue. The openFileDialogueOK should accept an array
         of file paths, The inital file name would be set to this main form, however, the rest of the file paths would be created using multiple forms*/
        private void openFileDialog_Read_FileOk(object sender, CancelEventArgs e)
        {
            //FileName is file name. FileNames is command

            FileName = openFileDialog_Read.FileName;//FileName is set to the the first filename read, this will be the main Filename
            int numberOfFiles = openFileDialog_Read.FileNames.Count();//OpenfileDialogue has a funciton which count the number of selected files
            StartofIt = dateTimePicker_start.Value.Date;//Returns the starting date from datetimepicker
            EndofIt = dateTimePicker_end.Value.Date;//Returns the ending date from the datetimepicker
            for (int i = 0; i< numberOfFiles; i++) { //For loop which itereates though all of the files selected. Uses integer i until it reaches count. But read files within FileNames array of i
                string CandlestickFilePath = openFileDialog_Read.FileNames[i];// read entire filepath of Candlestickfile
                string CandlestickFile = Path.GetFileNameWithoutExtension(CandlestickFilePath);//Reduce the path to just the filename
                if (i == 0)//True if we are looking at the first set of candlesticks to be set to the main form
                {
                    Form_Candlesticks form_Candlesticks = this; //The current or first candlestick is set as the main form candlestick using this
                    readandDisplayStock();//Calls on the function which reads through file and returns a filtered chart
                    form_Candlesticks.Text = "Parent: " + CandlestickFile;//Rename the form as a parent
                }
                else//Rest of the Candlesticks in for loop would be given their designated forms, and run using an object function call
                {
                    Form_Candlesticks form_Candlesticks = new Form_Candlesticks(CandlestickFilePath, StartofIt, EndofIt); //Create a Candlestick using parameters 
                    form_Candlesticks.readandDisplayStock();//Calls on the funciton which reads through file and returns filtered cnadles sticks but reads as object of created Candlesticks from parameters
                    form_Candlesticks.Text = "Child: " + CandlestickFile;//Renames the form as a child
                    form_Candlesticks.Show();//Display the created Child form
                }
            }



        }
        /*Function created by Form when you change or select a new element within the combobox as an event handler*/
        private void comboBox_CandleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionType = comboBox_CandleType.SelectedItem.ToString();//Returns the selected element from comboBox as a string
            smartDisplayChart();//Calls on the functions which will not only display the chart but also the annotations
        }
    }


    internal class Candlestick_Class
    {
        /*Each variable of the candlestick class is given a get and set function read and write access to properties. 
         without explicitly create a new function to do so. This is necessary for data binding and important that each component
        is public for access
         */
        public DateTime Date { get; set; } //Datetime variable for date is given get and set for read and write usability
        public decimal Open { get; set; } //decimal variable for open is given get and set for read and write usability
        public decimal Close { get; set; }//decimal variable for close is given get and set for read and write usability
        public decimal High { get; set; }//decimal variable for high is given get and set for read and write usability
        public decimal Low { get; set; }//decimal variable for low is given get and set for read and write usability
        public ulong Volume { get; set; }//Ulong variable (long int) for volume is given get and set for read and write usability

        /*public Candlestick_Class()   Default Constructor. 
        {
           
        }*/



        /// This is a constructer function for the Candlestick class, which creates candlesticks when gien a string
        /// It seperates the string into array and correspond the variable to the appropriate index of the array
        /// Additional condition for data types to match before assigning the variable
        public Candlestick_Class(string Row_Candlestick)
        {
            DateTime temp_date; //Tempoary variable for DateTime data types
            decimal temp_decimal;//Tempoary variable for decimal data types
            ulong temp_ulong;//Tempoary variables for ulong data types
            bool correct;//Empty bool to check each index of array matches with corresponding Candlestick variable
            char[] separators = new char[] { ',', ' ', '"' };// an array of characters, which is used to seperate the string by

            if (Row_Candlestick != null)//An if Condition allows the candlestick to be made as long as the string isn't empty
            {
                string[] Array_String = Row_Candlestick.Split(separators, StringSplitOptions.RemoveEmptyEntries);/* given a string it returns an array of strings sepearated by the characters from the serperator array
                StringSplitOptions.RemoveEmptyEntries remove empty entries or white spaces from the seperation*/

                /*Each variable is tested with theTryParse to ensure converting the index of array will matches with the data type of corresponding variable
                 Doing so will set boolean to true and enable the variable to be assigned the data. If False, will not continue*/

                correct = DateTime.TryParse(Array_String[0], out temp_date);//DateTime.Tryparse converts string[0] to DateTime. IF possible, correct is set to true and temp_date is DateTime variable
                if (correct) { Date = temp_date; }//if it was possible then set the tempoary matching variable to Date

                correct = decimal.TryParse(Array_String[1], out temp_decimal);//Decimal.Tryparse converts string[1] to Decimal. IF possible, correct is set to true and temp_decimal is decimal variable
                if (correct) { Open = temp_decimal; }//if it was possible then set the tempoary matching variable to Open

                correct = decimal.TryParse(Array_String[4], out temp_decimal);//Decimal.Tryparse converts string[4] to Decimal. IF possible, correct is set to true and temp_decimal is decimal variable
                if (correct) { Close = temp_decimal; }//if it was possible then set the tempoary matching variable to Close

                correct = decimal.TryParse(Array_String[2], out temp_decimal);//Decimal.Tryparse converts string[2] to Decimal. IF possible, correct is set to true and temp_decimal is decimal variable
                if (correct) { High = temp_decimal; }//if it was possible then set the tempoary matching variable to High

                correct = decimal.TryParse(Array_String[3], out temp_decimal);//Decimal.Tryparse converts string[3] to DateTime. IF possible, correct is set to true and temp_decimal is decimal variable
                if (correct) { Low = temp_decimal; }//if it was possible then set the tempoary matching variable to Low

                correct = ulong.TryParse(Array_String[6], out temp_ulong);//Ulong.Tryparse converts string[0] to Ulong. IF possible, correct is set to true and temp_ulong is ulong variable
                if (correct) { Volume = temp_ulong; }//if it was possible then set the tempoary matching variable to Volume

            }
        }
    }
    internal class smartCandlestick_Class: Candlestick_Class //you already done the inheritence
    {
        /*numbers to call on. Don't have to rewrite funciton. WIll have to initialize later on*/
        public decimal Range { get; set; } //Decimal type Range is given get/set for read/write capabilities
        public decimal Body{ get; set; }//Decimal type Body is given get/set for read/write capabilities
        public decimal TopTail { get; set; }//Decimal type Toptail is given get/set for read/write capabilities
        public decimal BottomTail { get; set; }//Decimal type Bottomtail is given get/set for read/write capabilities
        public decimal TopPrice { get; set; }//Decimal type TopPrice is given get/set for read/write capabilities
        public decimal BottomPrice { get; set; }//Decimal type BottomPrice si given get/set for read/write capabilities
        public Dictionary<string, bool> DictionaryCandles { get; set; }//Dictionary type DictionaryCandles is given get set for read write capabilities for the dictionary and it's elements

        /*base must already be constructed to call on from derive*/
        /*Invoker One Of the constructors of the base class pick constructor based on parameters/arguments*/


        /*All variables from the base class must already exist, which is job of the base class. PRIMARILY CALL any, but specify*/
        /*Constructor class for the smartcandlesticks which derives from the candlesticks constructor. Specifically it calls on the constructor with string parameters
         That way as it reads the properties of Candlesstick from file, it's also using thos properties to calculate the candlestick properties*/
        public smartCandlestick_Class(string Row_Candlestick) : base(Row_Candlestick) //Invoke the base class constructor --> object class
        {
            computeExtraParameters();//Function which set the SmartCandlestick Properties using the Candlestick Properties
            //computePatternProperties();//Initialize a dictionary, and within dictionary create key,values of candlestick type and existance to identify if this candlestick is the same type
        }
        /// <summary>
        /// Compute Parameters is a functions that uses the Candlestick Properties to calculate the SmartCandlestick Properties. This is possible
        /// because the function is called within the SmartCandlestick Constructor
        /// </summary>
        public void computeExtraParameters()
        {
            Range = High - Low; //Range is calculated by Subtracting the highest and lowest price
            Body = Math.Max(Open, Close) - Math.Min(Open, Close);//Body is calculated the range between open and close. Min and Max is necessary because location differentiate between bearish and boolish candlesticks
            TopPrice = Math.Max(Open, Close);//Topprice is whichever's the highest: open or close
            BottomPrice = Math.Min(Open, Close);//Bottom price is whichever's the loweset: open or close
            TopTail = High - TopPrice;//Toptail calculates the range of the line by subtracting the highest price with the edge of body
            BottomTail = BottomPrice - Low; //Bottomtail calculates the range of the line by subtracting the edge of the body with the lowest price
            DictionaryCandles = new Dictionary<string, bool>(); //Initialize Dictionary of string key and boolean value
        }
        /// <summary>
        /// This functions identifies if the candlestick identifies with a certain Candlestick type, by initalizing dictionary, and within dictionary use the smart
        /// candlestick properties to calculate each boolean variable to be included within the dictionary. To associate which candlestick type to the smart candlestick
        /// </summary>
        /*public void computePatternProperties()
        {
            bool isBullish = Close > Open; //returns true if close is greater than open
            bool isBearish = Close < Open;//returns true if open is greater than close
            bool isNeutral = Close == Open;//returns true if open is exactly the same value as close
            bool isMarubozu = Body >= ((decimal)(.80) * Range);//Returns true if majority of the candlestick is the body. About 80 percent of candlestick is just the body
            bool isHammer = Math.Abs(High-TopPrice)<= ((decimal)(.10)*Range) && ((decimal)(.15) * Range) < Body &&  Body < ((decimal)(.30) * Range);//returns true if the body is inbetween ranges to represent face of hammer and the topprice is close to highest price. This hammer doesn't care about handle
            bool isDoji = Body <= ((decimal)(.30) * Range);//returns true if candlestick body is less than 30 percent of the entire body
            bool isDragonflyDoji = isDoji && BottomTail >= ((decimal)(.50) * Range);//returns true if its doji and the body is in the upper half of candlestick
            bool isGravestoneDoji = isDoji && TopTail >= ((decimal)(.50) * Range);//returns true if its doji and the body is in the lower half of candlestick

            DictionaryCandles.Add("Bullish", isBullish); //add dictionary of bullish name and associated boolean
            DictionaryCandles.Add("Bearish", isBearish); //add dictinary of bearish name and assoiated boolean
            DictionaryCandles.Add("Neutral", isNeutral); // add dictionary of neutral namd and associated boolean
            DictionaryCandles.Add("Marubozu", isMarubozu); // add dictionary of Marubozu name and associated boolean
            DictionaryCandles.Add("Hammer", isHammer); // add dictionary of Hammer name and associated boolean
            DictionaryCandles.Add("Doji", isDoji); // add dictionary of Doji name and associated boolean
            DictionaryCandles.Add("DragonflyDoji", isDragonflyDoji);// add Dragonflydoji name and associated boolean
            DictionaryCandles.Add("GravestoneDoji", isGravestoneDoji);// add Gravestonedoji name and associated boolean
        }*/

    }
    /// <summary>
    /// This is an abstract class Recognizer, which acts as a framwork for other classes. There is no way you can directly 
    /// initialize the Recognizier class directly. However we can create multiple class with the same vairables and the all should have 
    /// the Recognize functionalities to inherit. 
    /// </summary>
    abstract class Recognizer
    {
        string patternName;
        int patternLength;
        public abstract bool Recognize(List<smartCandlestick_Class> Lcsc,int index) ; //Each subdicision class will have a recognizer class however, they will follow the same parameters as the abstract class, but run different functions 
        public Recognizer(string pn, int pl) //An empty abstract constructor which the candlestick Recgonizer have to inherit
        {
            patternName = pn; //Initialize PatternName based on given constructor
            patternLength = pl; //Initialize PatternLength based on given constructor
        }
        public void recognizeALL(List<smartCandlestick_Class> Lscs) //THis function is directly acessible by all subclass which allowes them to run their own subclass though all their cnadlesticks in the for loop
        {
            for (int i = 0; i < Lscs.Count; i++)// loop through candelsticks int he list
            {
                Recognize(Lscs, i);//each candlestick will run the associative Recognize function based on the index and the entire list
            }
        }
        public int returnLength {  get { return patternLength; } } //returnLength is a functions which uses get to return the patternlength of the Recognizer class
    }
    /// <summary>
    /// Recognizer Bullish inherits the Reconizer abstract class framework. Specifically the Recognize method and constructor
    /// Single Pattern Candlesticks Bullish --> rising single candlesticks
    /// </summary>
    internal class Recognizer_Bullish : Recognizer
    {
        public Recognizer_Bullish(string pn, int pl) : base(pn, pl) // Recongize constructor which inherits based parameters based on abstract clas 
        {
            // Additional initialization for Recognizer_Bullish if needed
        }
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Recognize and run their own Recognize function with same parameters
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is given the index from the list
            if (scs.DictionaryCandles.TryGetValue("Bullish", out bool value)) //The candlestick dictionary see if it has "Bullish" and if it return true, it returns the value 
            {
                return value;
            }
            else
            {
                bool r = scs.Close > scs.Open; //true if Close is higher than the Open price
                scs.DictionaryCandles.Add("Bullish", r);//Add a section in dicitonarry, to include bullish name and associated boolean 
                return r;//Also return the boolean
            }
        }
    }
    /// <summary>
    /// Recognizer Bearish inherits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Single Pattern candlestick bearish --> Falling single candlestick
    /// </summary>
    internal class Recognizer_Bearish : Recognizer
    {
        public Recognizer_Bearish(string pn, int pl) : base(pn, pl)
        { // Recongize constructor which inherits based parameters based on abstract class
            //Addtional Initilization for Recognizer_Bearish if needed
        }
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Overrid the abstract Recognize and run their own Recognize FUnction with the same parameters
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is give the index from the list
            if(scs.DictionaryCandles.TryGetValue("Bearish", out bool value) == false) //The candlestick dictionary see if it has "Bearish" and if it returns false, will add onto the dictionary
            {
                bool r = scs.Close < scs.Open; //True if Open is higher than the close price
                scs.DictionaryCandles.Add("Bearish", r); //Add a section in dictionary, to include bearish name and associated boolean
                return r;//also return the boolean
            }
            return scs.DictionaryCandles["Bearish"]; //if the condition returns true, it returns the value from the dictionary
        }

    }
    /// <summary>
    /// Recognizer Neutral inherits the Reocnizer abstarct class framework. Specifically the Recognize method and constructor
    /// Single Pattern candlestick neutral --> Candlestick with equal ope and close
    /// </summary>
    internal class Recognizer_Neutral: Recognizer
    {
        public Recognizer_Neutral(string pn, int pl) : base(pn, pl){}//Recognize constructor which inherits based parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Recognize and run their own Recognize function with the same parameters
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is the given index from the list
            if (scs.DictionaryCandles.TryGetValue("Neutral", out bool value) == false) //The candlestick Dictionary see if it has "Neutral" and if it returns false, and add onto the dictionary
            {
                bool r = scs.Close == scs.Open; //True if Open is equivalent to Close price
                scs.DictionaryCandles.Add("Neutral", r); //Add a section in Dictionary, to include neutral name and associated boolean
                return r;//also return the boolean
            }
            return scs.DictionaryCandles["Neutral"]; //if the condition returns true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer Marubozu inherits the Reocgnizer abstract class framework. Specifically teh Recognize method and constructor
    /// Single Pattern candlestick marubozu --> limit the length of the top and bottom tails
    /// </summary>
    internal class Recognizer_Marubozu: Recognizer
    {
        public Recognizer_Marubozu(string pn, int pl) : base(pn, pl) { } // Recognize constructor which inherits based paramters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Recognize and run their own Recognize function with the same paramters
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is the given index from the list
            if (scs.DictionaryCandles.TryGetValue("Marubozu", out bool value) == false) // The candlestick Dictionary see if it has "Marubozu" and if it returns false, and add onto the dictionary
            {
                bool r = scs.Body >= ((decimal)(.80) * scs.Range); //True if the body is equivalent to 80 percent of the range
                scs.DictionaryCandles.Add("Marubozu", r); //Add a section in Dectionary, to include Marubozu name and associated boolean 
                return r; //also return the boolean
            }
            return scs.DictionaryCandles["Marubozu"]; //.if the condition returns true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer Hammer inherits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Single Patter candlestick Hammer --> The body is small, but the bottom tail is large to create a hammer
    /// </summary>
    internal class Recognizer_Hammer: Recognizer
    {
        public Recognizer_Hammer(string pn, int pl) : base(pn,pl){ }//Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Recognize and run their own Recognize function with the same parameters
        {
            smartCandlestick_Class scs = Lcsc[index];//Smartcandlestick is given index from the list 
            if (scs.DictionaryCandles.TryGetValue("Hammer", out bool value) == false) // The candlestick Dictionary see if it has "Hammer" and if it returns false, it adds onto the dictionary
            {
                bool r = Math.Abs(scs.High - scs.TopPrice) <= ((decimal)(.10) * scs.Range) && ((decimal)(.15) * scs.Range) < scs.Body && scs.Body < ((decimal)(.30) * scs.Range); //The condition which return true if the candlestick looks like a hammer
                scs.DictionaryCandles.Add("Hammer", r); //Add a section in Dictionary, to include Hammer name and associated boolean
                return r; //also return the boolean
            }
            return scs.DictionaryCandles["Hammer"];// if the condution returns true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recongizer Doji inherits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Single pattern candlestick Doji --> The candlestick body is small the the top and bottom tail is larger
    /// </summary>
    internal class Recognizer_Doji: Recognizer
    {
        public Recognizer_Doji(string pn, int pl) : base(pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Reconize and run their own Recognize function with the same parameters
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is given index from the list
            if (scs.DictionaryCandles.TryGetValue("Doji", out bool value) == false) //The candlestick Dictionary see if it has "Doji" and if it returns false, it adds onto the dictionary
            {
                bool r = scs.Body <= ((decimal)(.30) * scs.Range);//return true if the body is 30 percent of the range
                scs.DictionaryCandles.Add("Doji", r); //Add a section in Dictionary, to include Doji name and associated boolean
                return r;//also return the boolean
            }
            return scs.DictionaryCandles["Doji"]; //if the condition returns true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recongizer DragonDoji inherits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Single pattern candlestick DragonflyDoji --> Candlestick body is small but the bottom is larger than top. to shape a dragonfly 
    /// </summary>
    internal class Recognizer_DragonflyDoji: Recognizer
    {
        public Recognizer_DragonflyDoji(string pn, int pl) : base(pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index)// Override the abstract Recognize and run their own Recognize function with the same paramters 
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick is given index from the list
            if (scs.DictionaryCandles.TryGetValue("DragonflyDoji", out bool value) == false)//The candlestick Dicitonary see if it has "dragonflyDoji" and if it returns false, it adds onto the dictionary
            {
                bool r = scs.Body <= ((decimal)(.30) * scs.Range) && scs.BottomTail >= ((decimal)(.50) * scs.Range);// return true if the body is 30 percent of range of Bottom tail is bigger than top tail
                scs.DictionaryCandles.Add("DragonflyDoji", r);//Add a section in Dictionary, to include DragonflyDoji name and associated boolean
                return r; //also return the boolean
            }
            return scs.DictionaryCandles["DragonflyDoji"]; //If the condition returns true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer GravestoneDoji inheirits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Signle patter candlestick GravestoneDoji --> Candlestick body is small but the top is larger then the bottom. to shape a gravestone
    /// </summary>
    internal class Recognizer_GravestoneDoji: Recognizer
    {
        public Recognizer_GravestoneDoji(string pn, int pl): base(pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) // Override the abstract Recognize and run their own Recognize function with the same paramters 
        {
            smartCandlestick_Class scs = Lcsc[index]; //Smartcandlestick it given index from the list
            if (scs.DictionaryCandles.TryGetValue("GravestoneDoji", out bool value) == false) //The candlestick Dictionary see if it has "GravestoneDoji" and if it returns false, it adds onto the dictionary
            {
                bool r = scs.Body <= ((decimal)(.30) * scs.Range) && scs.TopTail >= ((decimal)(.50) * scs.Range);// return true if the body is 30 percent of range of Top tail is bigger than bottom tail
                scs.DictionaryCandles.Add("GravestoneDoji", r); //Add a section in Dictionary, to include GravestoneDoji name and associated boolean
                return r;//also return the boolean
            }
            return scs.DictionaryCandles["GravestoneDoji"]; //If the condition return true, it returns the value from the dictionary
        }
    }

    /// <summary>
    /// Recognizer BullishEngulfing inherits the Recognizer abstract class framework. Specifically the Recognize method and constructor
    /// Two candlestick patter which trends to increasing candlesick from bearish to bullish
    /// </summary>
    internal class Recognizer_BullishEngulfing: Recognizer
    {
        public Recognizer_BullishEngulfing(string pn, int pl): base (pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Overrid the abstract Reocnize and run their own Recognize function with the same paramters
        {
            smartCandlestick_Class current = Lcsc[index]; //Smartcandlestick  is given index from the list
            if (index >= Lcsc.Count - 1) //The condition to check the index is at the end of the list. 
            {
                if (current.DictionaryCandles.TryGetValue("BullishEngulfing", out bool value2) == false) //if the ending candlestick not have a bullishengulfing entry it will create one and set it to false
                    current.DictionaryCandles.Add("BullishEngulfing", false);
                return false; //Regardless of the condition it will return false for edge candlesticks
            }
            smartCandlestick_Class next = Lcsc[index+1]; //Initialize the next smartcandlestick with current (index + 1)
            if (current.DictionaryCandles.TryGetValue("BullishEngulfing", out bool value) == false){//The condition which checks if dictionary has bullishEngulfing entry. Contnues if it was false
                bool r = (next.Close > next.Open) && (current.Close < current.Open) && //The condition which determines its goes bearish then bullish
                    (next.TopPrice > current.TopPrice) && (next.BottomPrice < current.BottomPrice); //Tthe condition which it trends small to bigger candlestick
                current.DictionaryCandles.Add("BullishEngulfing", r); //adds the Bullishengulfing string and boolean into dictionary
                return r;//return the boolean
            }
            return current.DictionaryCandles["BullishEngulfing"]; //If the condition return true, it returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer Bearish Engulfing inheirs the Recongizer abstarct class framework. Specifically the Recognize method and constructor
    /// Two candlestick pattern whic the trend increases from bullish to bearish
    /// </summary>
    internal class Recognizer_BearishEngulfing: Recognizer
    {
        public Recognizer_BearishEngulfing(string pn, int pl): base (pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index)//Overrid the abstract Reocnize and run their own Recognize function with the same paramters
        {
            
            smartCandlestick_Class current = Lcsc[index]; //Smartcandlestick is given index from the list
            if (index >= Lcsc.Count - 1) //The condition to check the index is at the end of the list
            {
                if (current.DictionaryCandles.TryGetValue("BearishEngulfing", out bool value2) == false) {  //if the ending candlestcik not have a BearishEngulfing entry, it will create one and set it to false
                    current.DictionaryCandles.Add("BearishEngulfing", false);
                }
                return false; //Regardless, the ending candlestick will always return false
            }
            smartCandlestick_Class next = Lcsc[index + 1]; //The smartcandlestick next is given the index +1 from the list
            if(current.DictionaryCandles.TryGetValue("BearishEngulfing", out bool value) == false) //The condition which checks if the candlestick doesn't have a bearish Engulfing entry, continue condition if false
            {
                bool r = (next.Close < next.Open) && (current.Close > current.Open)&& // The condition which determines if bullish goes before bearish
                    (next.TopPrice > current.TopPrice) && (next.BottomPrice < current.BottomPrice);// the condition which determines if teh cnadlestic goes from smaller to bigger
                current.DictionaryCandles.Add("BearishEngulfing", r);// Add into dictionarry the string and corresponding booleen from the condition
                return r;
            }
            return current.DictionaryCandles["BearishEngulfing"];//If the condition return true, ti returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer BullishHarmi inherits the Recognizer Abstract Class framework. Specifically the recognize method and constructor
    /// Two candlestcik pattern which the trend decreases from bearish to bullish
    /// </summary>
    internal class Recognizer_BullishHarami: Recognizer
    {
        public Recognizer_BullishHarami(string pn, int pl): base (pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Overrid the abstract Reocnize and run their own Recognize function with the same paramters
        {
            
            smartCandlestick_Class current = Lcsc[index];//Smartcandlestick is geiven index from the list
            if (index >= Lcsc.Count - 1) {//The condition to check the index is at the end of the list
                if (current.DictionaryCandles.TryGetValue("BullishHarami", out bool value2) == false) //iif the ending candlestick not have a BullishHarami entry, it will create one and set it to false
                    current.DictionaryCandles.Add("BullishHarami", false);
                return false; //Regardless, the ending candlestick will always return false
            }
            smartCandlestick_Class next = Lcsc[index + 1]; //The smartcandlestick next is given the index + 1 from the list
            if(current.DictionaryCandles.TryGetValue("BullishHarami", out bool value) == false) //The condition which chekcs if the candlestick doesn't have a bullishHarmi Entry, continue condition if false
            {
                bool r = (next.Close > next.Open) && (current.Close < current.Open) && //The condition which determines if bearish goes before bullish
                    (next.TopPrice < current.TopPrice) && (next.BottomPrice > current.BottomPrice); //the condition which determines if the candlestick goes from bigger to smaller
                current.DictionaryCandles.Add("BullishHarami", r);// Add into dictionary the string and corresponding boolean from the condition
                return r;
            }
            return current.DictionaryCandles["BullishHarami"];//If the condition return true, ti returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer BearishHarmi inherits the Recognizer Abstract Class framework. Specifically the recognize method and constructor
    /// Two candlestick pattern which the trend decerases from bullish to bearish
    /// </summary>
    internal class Recognizer_BearishHarami : Recognizer
    {
        public Recognizer_BearishHarami(string pn, int pl) : base(pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Overrid the abstract Reocnize and run their own Recognize function with the same paramters
        {
            
            smartCandlestick_Class current = Lcsc[index]; //Smartcandlestick is given index from the list
            if (index >= Lcsc.Count - 1) { //The condition to check the index is at the end of the list
                if (current.DictionaryCandles.TryGetValue("BearishHarami", out bool value2) == false) //If the ending candlestick not have a BearishHarmi entry, it will create one and set it to false
                    current.DictionaryCandles.Add("BearishHarami", false);
                return false; //Regardless, the ending candlestick will always return false
            }
            smartCandlestick_Class next = Lcsc[index + 1];//The smartcandlestick next is given teh index + 1 from the list
            if (current.DictionaryCandles.TryGetValue("BearishHarami", out bool value) == false)// The condition which checks if the candlestick doesn't have a Bearishharami Entry, continue consition if it's false
            {
                bool r = (next.Close < next.Open) && (current.Close > current.Open) && // the bullish should come bfore the bearish
                    (next.TopPrice < current.TopPrice) && (next.BottomPrice > current.BottomPrice);// The trend should go from bigger to smaller
                current.DictionaryCandles.Add("BearishHarami", r); //Add into dictionary the string and corrresponding boolean from the condition
                return r;
            }
            return current.DictionaryCandles["BearishHarami"];//If the condition return true, ti returns the value from the dictionary
        }
    }

    /// <summary>
    /// Recognizer Peak inherits recognizer Abstract class framework. Specifically the recognize method and the constructor
    /// Three candlestick pattern which the middle candlestick is highest from its previous and next candlestick. Creates a hill
    /// </summary>
    internal class Recognizer_Peak : Recognizer
    {
        public Recognizer_Peak(string pn, int pl) : base(pn, pl) //Recognize constructor which inherits based on parameters on the abstract class
        {
            // Additional initialization for Recognizer_Bullish if needed
        }
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Overrid the abstract Reocnize and run their own Recognize function with the same paramters
        {
            smartCandlestick_Class scs = Lcsc[index];//Smart Candlestick is given the index from the list
            if(index <= 0 || index >= Lcsc.Count - 1) // The condition to check if the candlestick is at the beginning or the end of the list
            {
                if (scs.DictionaryCandles.TryGetValue("Peak", out bool value2) == false) //The condition to check if the candlestick has an entry Peak in the dectionary.
                    scs.DictionaryCandles.Add("Peak", false);
                return false;//If it doesn't it will add the entry with string and false into the dictionary and return false
            }
            if(scs.DictionaryCandles.TryGetValue("Peak", out bool value) == false)//The condition to check if the candlestick has an etnriy peak in the dictionary. If not continue through the condition
            {
                decimal previous = Lcsc[index - 1].High;
                decimal current = Lcsc[index].High;
                decimal next = Lcsc[index + 1].High;
                bool r = (previous < current) && (current > next); // return boolean if the current smartCandlestick current.high is higher than the preivous.high and next.high 
                scs.DictionaryCandles.Add("Peak", r); //Add into dictionary with the name of the patter with the corresponding boolean
                return r; // return the boolean 
            }
            return scs.DictionaryCandles["Peak"];//If the condition return true, ti returns the value from the dictionary
        }
    }
    /// <summary>
    /// Recognizer Valley inherits the recognizer abstract class framework. Specifically the reconize method and the constructor
    /// Three candlestick pattern which the middle candlestick is lowes from its previous and next candlestic. Creates a pit
    /// </summary>
    internal class Recognizer_Valley: Recognizer
    {
        public Recognizer_Valley(string pn, int pl): base (pn, pl) { } //Recognize constructor which inherits based on parameters on the abstract class
        public override bool Recognize(List<smartCandlestick_Class> Lcsc, int index) //Override the abstract Reocnize and run their own Recognize function with the same paramters
        {
            smartCandlestick_Class scs = Lcsc[index];//Smart Candlestick is given the index from the list
            if (index <= 0 || index >= Lcsc.Count - 1)//The conditions which check if the candlestick was at the beginning or end of the list
            {
                if (scs.DictionaryCandles.TryGetValue("Valley", out bool value2) == false) //The condition which checks if the edge candlesticks have a Valley entry in their dictionaries
                    scs.DictionaryCandles.Add("Valley", false);
                return false; //If they don't they have an entry the if condition will add an entry with Valley string and false boolean into dictionary and return false
            }
            if(scs.DictionaryCandles.TryGetValue("Valley", out bool value) == false) //Condition which chekc if the candlestick has an entry Valley in the dictionary. If not, goes through the if condition
            {
                decimal previous = Lcsc[index - 1].Low;
                decimal current = Lcsc[index].Low;
                decimal next = Lcsc[index + 1].Low;
                bool r = (previous > current) && (current < next); //return boolean if the current smartCandlestick current.low is lower than the previous.low and the next.low
                scs.DictionaryCandles.Add("Valley", r); //Add into dictionary with the name of the pattern with the corresponding boolean
                return r; //Return the boolean
            }
            return scs.DictionaryCandles["Valley"];//If the condition return true, ti returns the value from the dictionary
        }
    }

       



    }
