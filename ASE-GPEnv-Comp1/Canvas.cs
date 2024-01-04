﻿using ASE_GPEnv_Comp1.ShapesClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASE_GPEnv_Comp1
{
    public struct PenPosition
    {
        public int posX;
        public int posY;

    
        public PenPosition(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public void setDefaultPosition() {
            posX = 0;
            posY = 0;
        }
    }
    public class Canvas
    {
        Pen pen;
        //float penWidth;
        //Color penColor;
        PenPosition penPosition;

        Graphics graphics;
        Panel canvasPanel;
        RichTextBox commandsHistoryTextBox;
        RichTextBox outputTextBox;
        TextBox commandInputTextBox;
        RichTextBox programTextBox;


        bool hasInitializedPosition = false;
        public bool shouldFill= false;


        public Canvas(Color penColor, float penWidth, Panel panel, RichTextBox commandsHistoryTextBox, RichTextBox outputTextBox, TextBox commandInputTextBox, RichTextBox programTextBox)
        {
            this.pen = new Pen(Color.Red, penWidth);
            this.canvasPanel = panel;
            this.graphics = panel.CreateGraphics();
            this.penPosition =new PenPosition();
            this.penPosition.setDefaultPosition();

            this.commandsHistoryTextBox = commandsHistoryTextBox;
            this.outputTextBox = outputTextBox;
            this.commandInputTextBox = commandInputTextBox;
            this.programTextBox = programTextBox;
        }


        public void moveTo(PenPosition  penPosition)
        {
            this.hasInitializedPosition = true;
            this.penPosition = penPosition;
        }
 
        public void moveTo(int posX, int posY)
        {
            this.hasInitializedPosition = true;
            this.penPosition = new PenPosition(posX, posY);

        }

        public void drawCurrentPosition(PenPosition position) {

            int radius = 1;
            this.graphics.DrawEllipse(pen, position.posX - radius, position.posY - radius, 2 * radius, 2 * radius);

        }
        public void drawTo()
        {

            if (!hasInitializedPosition)
            {
                Debug.WriteLine("Waring! No position defined, setting position to (0,0)");
                this.appendExecutionResultsToOutput("Waring! No position defined, setting position to (0,0)");
                this.penPosition.setDefaultPosition();
            }
            drawCurrentPosition(this.penPosition);
        }
            
        public void drawTo(PenPosition penPosition)
        {
            drawCurrentPosition(penPosition);

        }

        public void clearCanvas() {
            this.graphics.Clear(canvasPanel.BackColor);

        }

        public void resetPen() {
            this.penPosition.setDefaultPosition();

        }

        public void drawRectangle(int width, int height)
        {
            if (shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillRectangle(fillingBrush, this.penPosition.posX, this.penPosition.posY, width, height);
            }
            this.graphics.DrawRectangle(this.pen, this.penPosition.posX, this.penPosition.posY, width, height);

        }
        public void drawRectangle(ShapesClasses.Rectangle rectangle)
        {
            int width = rectangle.width;
            int height = rectangle.height;
            int posX = rectangle.posX;
            int posY = rectangle.posY;

            Color rectangleColor = rectangle.shapeColor;
            this.setPenColor(rectangleColor);

            if (shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillRectangle(fillingBrush, posX, posY, width, height);
            }
            this.graphics.DrawRectangle(this.pen, posX, posY, width, height);

        }

        public void drawCircle(int radius)
        {
            int translatedX = this.penPosition.posX - radius;
            int translatedY = this.penPosition.posY - radius;

            if(shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillEllipse(fillingBrush, translatedX, translatedY, 2 * radius, 2 * radius);
            }
            else
                this.graphics.DrawEllipse(pen, translatedX, translatedY, 2 * radius, 2 * radius);

        }
        public void drawCircle(Circle circle)
        {
            int radius = circle.radius;
            int translatedX = circle.posX - radius;
            int translatedY = circle.posY - radius;
            Color circleColor = circle.shapeColor;
            this.setPenColor(circleColor);

            if (shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillEllipse(fillingBrush, translatedX, translatedY, 2 * radius, 2 * radius);
            }
            else
                this.graphics.DrawEllipse(this.pen, translatedX, translatedY, 2 * radius, 2 * radius);

        }


        public void drawTriangle(int sideLength)
        {

            int halfSide = sideLength / 2;
       
            Point firstPoint = new Point(this.penPosition.posX, this.penPosition.posY - halfSide);
            Point secondPoint = new Point(this.penPosition.posX + halfSide, this.penPosition.posY);
            Point thirdPoint = new Point(this.penPosition.posX - halfSide, this.penPosition.posY);


            if (shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillPolygon(fillingBrush, new Point[] { firstPoint, secondPoint, thirdPoint });

            }
            else
            {
                this.graphics.DrawLine(pen, firstPoint, secondPoint);
                this.graphics.DrawLine(pen, secondPoint, thirdPoint);
                this.graphics.DrawLine(pen, thirdPoint, firstPoint);
            }


        }
        public void drawTriangle(Triangle triangle)
        {

            int halfSide = triangle.sideLength / 2;
            Color triangleColor = triangle.shapeColor;
            this.setPenColor(triangleColor);

            Point firstPoint = new Point(triangle.posX, triangle.posY - halfSide);
            Point secondPoint = new Point(triangle.posX + halfSide, triangle.posY);
            Point thirdPoint = new Point(triangle.posX - halfSide, triangle.posY);


            if (shouldFill)
            {
                Brush fillingBrush = new SolidBrush(this.pen.Color);
                this.graphics.FillPolygon(fillingBrush, new Point[] { firstPoint, secondPoint, thirdPoint });

            }
            else
            {
                this.graphics.DrawLine(pen, firstPoint, secondPoint);
                this.graphics.DrawLine(pen, secondPoint, thirdPoint);
                this.graphics.DrawLine(pen, thirdPoint, firstPoint);
            }


        }


        public void setPenColor(Color newColor) {
            this.pen.Color = newColor;
        }

        public void setPenFill(bool shouldFill)
        {
            this.shouldFill = shouldFill;
        }



        public void appendCommandToHistory(string text, bool isSuccess) {

            Color color = isSuccess ? Color.Green : Color.Red;

            text = (isSuccess? "[SUCCESS]- ":"[FAILURE]-   ")+ text + "\n";
            this.commandsHistoryTextBox.SelectionStart = commandsHistoryTextBox.TextLength;
            this.commandsHistoryTextBox.SelectionLength = 0;

            this.commandsHistoryTextBox.SelectionColor = color;
            this.commandsHistoryTextBox.AppendText(text);
            this.commandsHistoryTextBox.SelectionColor = commandsHistoryTextBox.ForeColor;
            this.commandsHistoryTextBox.ScrollToCaret();


        }
        public void appendExecutionResultsToOutput(string text) {
            text = text + "\n";
            this.outputTextBox.AppendText(text);
            this.outputTextBox.ScrollToCaret();

        }

        public void clearCommandInputBox()
        {
            this.commandInputTextBox.Clear();
        }

        public Pen getPen() {
            return this.pen;
        }
        public PenPosition getPenPosition()
        {
            return this.penPosition;
        }


        public string getProgramFromEditor() {
            return this.programTextBox.Text;

        }

        /// <summary>
        /// This function is used by handleLoadProgram. 
        /// It's responsible for extracting the file content. 
        /// </summary>
        /// <param name="selectedFilePath">Path to load program from</param>
        /// <returns>Content of file selected.</returns>
        public string readProgramFromFile(string selectedFilePath) {

            try
            {
                return File.ReadAllText(selectedFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Opens a dialog box and asks user to select a file
        /// This ONLY ALLOWS GPL extension file
        /// </summary>
        public void handleLoadProgram() {
            OpenFileDialog openFileBrowser = new OpenFileDialog();

            openFileBrowser.Title = "Browse for a .gpl file";
            openFileBrowser.Filter = "Graphics Programming Language files (.gpl)|*.gpl";

            if (openFileBrowser.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileBrowser.FileName;

                if (Path.GetExtension(selectedFilePath).Equals(".gpl"))
                {
                    try
                    {
                        string graphicsProgram = readProgramFromFile(selectedFilePath);
                        this.programTextBox.Text = graphicsProgram;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading the file. Try other file." + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a text file with the .txt extension.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// This function directly saves the content of program editor box to selected file path.
        /// </summary>
        /// <param name="gplFilePath">File path to store program in</param>
        /// <param name="graphicsProgram">Program to store</param>
        public void storeProgramToFile(string gplFilePath, string graphicsProgram) {
            try
            {
                File.WriteAllText(gplFilePath, graphicsProgram);
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Opens the dialog box and asks the user to select a filename with path to store program.
        /// </summary>
        public void handleSaveProgram() {
            SaveFileDialog saveBrowser = new SaveFileDialog();

            saveBrowser.Title = "Save to a .gpl file";
            saveBrowser.Filter = "Graphics Programming Language files (.gpl)|*.gpl"; ;



            if (saveBrowser.ShowDialog() == DialogResult.OK)
            {
                string gplFilePath = saveBrowser.FileName;

                try
                {
                    string graphicsProgram = programTextBox.Text;
                    storeProgramToFile(gplFilePath, graphicsProgram);
                   

                    MessageBox.Show("Program saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving program file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }
}
