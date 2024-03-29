﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASE_GPEnv_Comp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASE_GPEnv_Comp1.Exceptions;
using static ASE_GPEnv_Comp1.CommandParser;
using System.Windows.Forms;

namespace ASE_GPEnv_Comp1.Tests
{
    [TestClass()]
    public class CommandParserTests
    {
        public string failedTestMessage(ParsingException ex)
        {
            string message = ex.Message + " " + ex.getParsingExceptionMessage();
            message = message + "Test Failed! Exception should not be raised. Message: ";
            return message;
        }

        /// <summary>
        /// Unit test to check if one valid command executes.
        /// Without any exception. 
        /// Example Command: drawto 100,100
        /// Expected Behaviours: Draw the cursor on canvas without any error
        /// Generated Result: Drawn the curson on canvas without any error. 
        /// Test Status: Passed
        /// </summary>
        [TestMethod()]
        public void executeOneCommandTest_ValidCommandTest()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand("drawto 100, 100", -1);
                Assert.IsTrue(parsingResult.isSuccessful);

            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));


            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }


        /// <summary>
        /// Unit test to check if mulitple valid commands / Program executes.
        /// Without any exception. 
        /// Example Program: 
        ///     moveto 100,100
        ///     pen cyan
        ///     rectangle 100,100
        ///     fill on
        ///     pen yellow
        ///     circle 20
        /// Expected Behaviours: Draw the shapes on the canvas. 
        /// Generated Result: Drawnn the shapes on canvas without any error. 
        /// Test Status: Passed
        /// </summary>
        /// 
        [TestMethod()]
        public void executeWholeProgramTest_ValidProgramTest()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);

            string testProgram = @"
            moveto 100,100
            pen cyan
            rectangle 100,100
            fill on
            pen yellow
            circle 20";
            mainUI.setProgramText(testProgram);

            try
            {
                bool hasPassedAll = true;
                List<ParsingInfo> parsingResults = parser.executeWholePrograme(testProgram);
                foreach (ParsingInfo parsingResult in parsingResults)
                {
                    if (!parsingResult.isSuccessful)
                    {
                        hasPassedAll = false;
                        break;
                    }
                }
                Assert.IsTrue(hasPassedAll);

            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));

            }
            catch (InvalidParamsException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command params", failedTestMessage(ex));
            }
            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }


        /// <summary>
        /// Unit test to check if system is able to catch the invalid command exceptions on Invalid commands
        /// Without any exception. 
        /// Example Invalid Commands: 
        ///     invalid
        ///     crcle  50
        ///     moveto 100,100
        /// Expected Behaviours: Catch the invalid command exception. 
        /// Generated Result: Catched the invalid command exception.
        /// Test Status: Passed
        /// </summary>
        [TestMethod()]
        public void checkSyntaxTest_InvalidCommands()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            //mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);

            string[] invalidTestCommands = {
                "invalid",
                "crcle 50",
                "movto 100,100"
            };
            foreach (string invalidCommand in invalidTestCommands)
            {
                try
                {
                    ParsingInfo info = parser.checkSyntax(invalidCommand, -1);
                    foreach (ParsingException parsingException in info.parsingExceptions)
                    {
                        throw parsingException;
                    }
                    Assert.Fail("Test Failed for " + invalidCommand);

                }
                catch (InvalidCommandException ex)
                {
                    StringAssert.Contains(ex.Message.ToLower(), "invalid command");

                }
            }
        }

        /// <summary>
        /// Unit test to check if system is able to catch the invalid command pararms exceptions on Invalid command parameters
        /// Example Invalid Params: 
        ///     circle x
        ///     moveto 100
        ///     drawto 100,100,100
        /// Expected Behaviours: Catch the relevant params exception. 
        /// Generated Result:   Catched the relevant params exception.
        /// Test Status: Passed
        /// </summary>
        [TestMethod()]
        public void checkSyntaxTest_InvalidCommandParams()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            //mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);

            string[] invalidTestCommandParams = {
                "circle x",
                "moveto 100",
                "drawto 100,100,100"
            };
            foreach (string invalidCommandParam in invalidTestCommandParams)
            {
                try
                {
                    ParsingInfo info = parser.checkSyntax(invalidCommandParam, -1);
                    foreach (ParsingException parsingException in info.parsingExceptions)
                    {
                        throw parsingException;
                    }
                    Assert.Fail("Test Failed for " + invalidCommandParam);

                }
                catch (InvalidParamsException ex)
                {

                    if (invalidCommandParam == "circle x")
                        StringAssert.Contains(ex.Message.ToLower(), "invalid param type");

                    else if (invalidCommandParam == "moveto 100")
                        StringAssert.Contains(ex.invalidParamsMessage.ToLower(), "insufficient param");
                    else
                        StringAssert.Contains(ex.invalidParamsMessage.ToLower(), "too much param");


                }
            }
        }


        /// <summary>
        /// Unit test for moveto command, to check if cursor moves successfully
        /// Example Command: 
        ///     moveto 100,100
        /// Expected Behaviours: Change the pen position . 
        /// Generated Result: Changed the pen position without any error.
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_moveTo()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "moveto 100, 100";

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));
            
            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }



        /// <summary>
        /// Unit test for drawto command, to check if position of pen is updated with cursor visualized
        /// Example Command: 
        ///     drawto 200,200
        /// Expected Behaviours: Change the pen position and draw the cursor position. 
        /// Generated Result: Changed the pen position and drawn cursor without any error.
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_drawTo()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "drawto 200, 200";

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }



        /// <summary>
        /// Unit test for clear command, to check if the canvas is cleared by said command 
        /// WIHOUT loosing the current pen position for future drawings
        /// Example Commands: 
        ///     moveto 20,20
        ///     rectangle 200,200
        ///     clear
        ///     rectangle 200,200
        /// Expected Behaviours: Clear all drawings on canvas and preserve the position . 
        /// Generated Result: Cleared all drawings on canvas and preserved the position for next rectangle.
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_clear()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "clear";

            MessageBox.Show("Drawing a rectangle of 200,200 at 20,20");
            parser.executeOneCommand("moveto 20, 20", -1);
            parser.executeOneCommand("rectangle 200, 200", -1);


            try
            {
                MessageBox.Show("Clearing Canvas now.");
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }


            MessageBox.Show("To check position reset redrawing a rectangle of 200,200");
            parser.executeOneCommand("rectangle 200, 200", -1);


            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }



        /// <summary>
        /// Unit test for reset command, to check if pen position resets to default for future drawings
        /// Example Commands: 
        ///     moveto 20,20
        ///     rectangle 200,200
        ///     clear
        ///     reset
        ///     rectangle 200,200
        /// Expected Behaviours: Reset the positon of pen for future drawings drawings on canvas. 
        /// Generated Result: Changed the position of pen for furture drawings to default.
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_reset()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "reset";

            MessageBox.Show("Drawing a rectangle of 200,200 at 20,20");
            parser.executeOneCommand("moveto 20, 20", -1);
            parser.executeOneCommand("rectangle 200, 200", -1);


            try
            {
                MessageBox.Show("Clearing Cursor and Canvas now.");
                parser.executeOneCommand("clear",-1);
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }


            MessageBox.Show("To check position reset redrawing a rectangle of 200,200");
            parser.executeOneCommand("rectangle 200, 200", -1);


            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }


        /// <summary>
        /// Unit test for rectangle command, to check if command draws the rectangle of width and height set by user.
        /// Example Commands:
        ///     rectangle 50,100
        /// Expected Behaviours: Draw a rectangle of width 50 and height 100 on canvas
        /// Generated Result:Drawn a rectangle of width 50 and height 100 on canvas
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_rectangle()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "rectangle 50, 100";

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }


        /// <summary>
        /// Unit test for circle command, to check if command draws the circle of radius set by user.
        /// Example Commands:
        ///     rectangle 50
        /// Expected Behaviours: Draw a circle of radius on canvas
        /// Generated Result:Drawn a circle of radius 50 on canvas
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_circle()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            string command = "circle 50";

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }



        /// <summary>
        /// Unit test for triangle command, to check if command draws the triangle of side length set by user.
        /// Example Commands:
        ///     moveto 100,100
        ///     triangle 50
        /// Expected Behaviours: Draw a triangle of each side length 50 on canvas
        /// Generated Result:Drawn a triangle of each side length 50 on canvas
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_triangle()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
            MessageBox.Show("Moving cursor to 100,100 and drawing triangle of side length 50");
            parser.executeOneCommand("moveto 100,100",-1);
            string command = "triangle 50";

            try
            {
                ParsingInfo parsingResult = parser.executeOneCommand(command, -1);
                Assert.IsTrue(parsingResult.isSuccessful);
            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }




        /// <summary>
        /// Unit test for pen {color} command, to check if command changes the color of pen/brush for future drawings
        /// Example Commands:
        ///     pen black
        ///     rectangle 50,50
        ///     pen red
        ///     triangle 80,80
        /// Expected Behaviours: Draw a black rectangle then a red rectangle
        /// Generated Result:Drawn a black rectangle then a red rectangle
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_penColor()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);
         
            string command1 = "pen black";
            string command2 = "pen red";

            try
            {
                ParsingInfo parsingResult1 = parser.executeOneCommand(command1, -1);
                Assert.IsTrue(parsingResult1.isSuccessful);
                parser.executeOneCommand("rectangle 50,50", -1);

                MessageBox.Show("Changing Color to Red and Drawing another triangle of differnt size.");
                ParsingInfo parsingResult2 = parser.executeOneCommand(command2, -1);
                Assert.IsTrue(parsingResult2.isSuccessful);
                parser.executeOneCommand("rectangle 80,80", -1);

            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }


        /// <summary>
        /// Unit test for fill {on|off} command, to check if command changes the fill mode of pen/brush for future drawings
        /// Example Commands:
        ///     fill on
        ///     rectangle 50,50
        ///     fill off
        ///     triangle 80,80
        /// Expected Behaviours: Draw a filled rectangle then a outlined rectangle
        /// Generated Result:Drawn a filled rectangle then a outlined rectangle
        /// Test Status: Passed
        /// </summary>
        [TestMethod]
        public void executeOneCommandTest_penFill()
        {
            MainUI_AseGPL1 mainUI = new MainUI_AseGPL1();
            mainUI.Visible = true;
            CommandParser parser = new CommandParser(mainUI.canvas, mainUI.clearTextCB);

            string command1 = "fill on";
            string command2 = "fill off";

            try
            {
                ParsingInfo parsingResult1 = parser.executeOneCommand(command1, -1);
                Assert.IsTrue(parsingResult1.isSuccessful);
                parser.executeOneCommand("rectangle 50,50", -1);

                MessageBox.Show("Changing fill mode to unfill and Drawing another triangle of differnt size.");
                ParsingInfo parsingResult2 = parser.executeOneCommand(command2, -1);
                Assert.IsTrue(parsingResult2.isSuccessful);
                parser.executeOneCommand("rectangle 80,80", -1);

            }
            catch (InvalidCommandException ex)
            {
                StringAssert.Contains(ex.getParsingExceptionMessage().ToLower(), "invalid command", failedTestMessage(ex));
                Assert.Fail(failedTestMessage(ex));

            }

            // added this to see the result of execution on screen
            MessageBox.Show("Test Completed");
        }






    }
}
