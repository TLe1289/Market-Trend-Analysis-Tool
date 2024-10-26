# Market-Trend-Analysis-Tool
The Market-Trend-Analysis-Tool is a .NET Framework application that allows users to view candlestick charts of selected stocks by extracting yearly, monthly, and daily trends from the Stock_Data Excel file. Users can display multiple stocks, select specific patterns, and adjust the time frame for each stock. Additionally, the tool includes a feature for identifying stock patterns directly from the chart, making it easier for users to spot trends and make informed buy or sell decisions.

## Software Setup and Running the Program
1. **Download and Install Visual Studio Code**  
   Ensure that the ".NET Framework" packages are included during setup.
2. **Open the Project**  
   Navigate to the `RecognizerCandlesticks` directory and open the `CorrectedCandlesticks.sln` file.
3. **Run the Program**  
   Click on the green "Run" triangle at the top to start the program.
4. **Load Chart Data**  
   - A display window should appear. Click **Load Chart**.
   - Navigate to the "Stock_Data" directory and select it.
   - Choose one or more candlestick chart files, which should display automatically.

## Program Files and Dependencies
1. **Form_Candlesticks.Designer.cs**  
   This file defines the visual layout and elements of the candlestick analysis interface, including various modules and tool classes—such as buttons, charts, and input fields—that make up the program's interface.
2. **Form_Candlesticks.cs**  
   The main program that provides functionality for the visual layout and elements defined in the designer class, enabling interaction and responsiveness within the interface. Key components include:
   - **Candlestick Class**: Represents each row of the Excel file, detailing components of each candlestick displayed in the chart. This also includes the volume graph for the bar chart below.
   - **Recognizer Class**: Analyzes a sequence of candlesticks to identify patterns based on relative pricing, pinpointing the candlestick or location where the pattern originates.
3. **Form_Candlesticks.cs [Design]**  
   Previews the webpage design based on modules or tool classes created in `Form_Candlesticks.Designer.cs`. For instance, a button class created in `Form_Candlesticks.Designer.cs` will display as a button in the preview section, allowing users to see how each component will appear on the page.

## Program Sequence: Read from Excel file and display "Candlesticks"
1. `readCandlesticksFromFile();` // Reads data from the specified file path and transfers all candlestick data into a `SmartCandlestick` list.
2. `filterCandlesticks();` // Filters candlesticks within a specified date range, creating a new list of candlesticks that fall within that range.
3. `normalizeChart();` // Calculates the minimum and maximum Y-axis values for the candlestick chart based on the filtered list.
4. `displayChart();` // Loads the filtered candlestick list into a BindingList and uses it to display data in a DataGridView and on the chart.

