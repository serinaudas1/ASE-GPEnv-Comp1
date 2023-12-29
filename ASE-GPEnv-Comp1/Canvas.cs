﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    class Canvas
    {
        Pen pen;
        float penWidth;
        Color penColor;
        PenPosition penPosition;

        Graphics graphics;
        Panel canvasPanel;

        bool hasInitializedPosition = false;


        public Canvas(Color penColor, float penWidth, Panel panel)
        {
            this.pen = new Pen(Color.Red, penWidth);
            this.canvasPanel = panel;
            this.graphics = panel.CreateGraphics();
            this.penPosition =new PenPosition();
            this.penPosition.setDefaultPosition();
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

        public void drawTo() {

            if (!hasInitializedPosition)
            {
                Debug.WriteLine("Waring! No position defined, setting position to (0,0)");
                this.penPosition.setDefaultPosition();
            }

            int radius = 5;
            this.graphics.DrawEllipse(pen, this.penPosition.posX - radius, this.penPosition.posY - radius, 2 * radius, 2 * radius);
        }

        public void clearCanvas() {
            this.graphics.Clear(canvasPanel.BackColor);

        }

        public void resetPen() {
            this.penPosition.setDefaultPosition();

        }


    }
}
