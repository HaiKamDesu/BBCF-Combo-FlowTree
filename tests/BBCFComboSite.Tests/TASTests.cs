using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCFComboSite.Tests
{
    class TASTests
    {
        [Test] // remove attribute if you don't use NUnit
        public void RunFullTest()
        {
            // Adjust path as you like
            var path = @"D:\User\Downloads\Kokonoe\FullTASRundown.ahk";

            BBCF_TAS.CreateScript(path, cfg =>
            {
            })
            .WaitUntil().Show("Reset").Reset()
            .WaitUntil().Show("Wait 1s").WaitFrames(60)
            

            //Reset tests
            .WaitUntil().Show("Back To Corner Reset").BackToCornerReset().WaitFrames(60)
            .WaitUntil().Show("Midscreen Reset").MidscreenReset().WaitFrames(60)
            .WaitUntil().Show("Corner Reset").CornerReset().WaitFrames(60)
            .WaitUntil().Show("Reset").Reset().WaitFrames(60)
            .WaitUntil().Show("Midscreen Reset").MidscreenReset().WaitFrames(60)


            // Movement tests
            .WaitUntil().Show("Run (hold 30f)").Run(30).WaitFrames(10)
            .WaitUntil().Show("Jump").Jump().WaitFrames(10)
            .WaitUntil().Show("Superjump 29")._29().WaitFrames(30)
            .WaitUntil().Show("Superjump 28")._28().WaitFrames(30)
            .WaitUntil().Show("Superjump 27")._27().WaitFrames(30)

            // Facing flip test
            .WaitUntil().BackToCornerReset().WaitFrames(30).Jump().WaitFrames(5).ForwardDash().WaitFrames(20).Jump().WaitFrames(5).ForwardDash()
            .Show("Face Left").FaceLeft().WaitFrames(10)
            .Show("Backdash (Facing Left)").BackDash()
            .WaitUntil().Show("Run (hold 20f) Facing Left").Run(20)
            .WaitUntil().MidscreenReset()
            .Show("Face Right").FaceRight()
            
            // Neutral buttons
            .WaitUntil().Show("5A")._5A().WaitFrames(30)
            .WaitUntil().Show("5B")._5B().WaitFrames(30)
            .WaitUntil().Show("5C")._5C().WaitFrames(30)
            .WaitUntil().Show("5D")._5D().WaitFrames(30)
            .WaitUntil().Show("F1")._F1().WaitFrames(30)
            .WaitUntil().Show("F2")._F2().WaitFrames(30)
            
            // Directional normals
            .WaitUntil().Show("6A")._6A().WaitFrames(30)
            .WaitUntil().Show("6B")._6B().WaitFrames(30)
            .WaitUntil().Show("6C")._6C().WaitFrames(30)
            .WaitUntil().Show("6D")._6D().WaitFrames(30)
            
            .WaitUntil().Show("2A")._2A().WaitFrames(30)
            .WaitUntil().Show("2B")._2B().WaitFrames(30)
            .WaitUntil().Show("2C")._2C().WaitFrames(30)
            .WaitUntil().Show("2D")._2D().WaitFrames(30)
            
            .WaitUntil().Show("3C")._3C().WaitFrames(30)
            
            // Full numpad D
            .WaitUntil().Show("1D")._1D().WaitFrames(30)
            .WaitUntil().Show("2D")._2D().WaitFrames(30)
            .WaitUntil().Show("3D")._3D().WaitFrames(30)
            .WaitUntil().Show("4D")._4D().WaitFrames(30)
            .WaitUntil().Show("5D")._5D().WaitFrames(30)
            .WaitUntil().Show("6D")._6D().WaitFrames(30)
            .WaitUntil().Show("7D")._7D().WaitFrames(30)
            .WaitUntil().Show("8D")._8D().WaitFrames(30)
            .WaitUntil().Show("9D")._9D().WaitFrames(30)
            
            // System/macros
            .WaitUntil().Show("Throw (B+C)").Throw().WaitFrames(30)
            .WaitUntil().Show("Rapid Cancel (A+B+C)").RapidCancel().WaitFrames(30)
            .WaitUntil().Show("Counter Assault (6+A+B)").CounterAssault().WaitFrames(30)
            .WaitUntil().Show("Overdrive (A+B+C+D)").Overdrive().WaitFrames(30)
            
            // Specials / Distortions (Kokonoe)
            .WaitUntil().Show("Activate (236D)").Activate().WaitFrames(30)
            .WaitUntil().Show("Retrieve (214D)").RetrieveGraviton().WaitFrames(30)
            
            .WaitUntil().Show("Broken Bunker A (236A)").BrokenBunkerA().WaitFrames(30)
            .WaitUntil().Show("Solid Wheel (236B)").SolidWheel().WaitFrames(30)
            .WaitUntil().Show("Absolute Zero (236C)").AbsoluteZero().WaitFrames(30)
            
            .WaitUntil().Show("Flame Cage A (214A)").FlameCageA().WaitFrames(30)
            .WaitUntil().Show("Flame Cage B (214B)").FlameCageB().WaitFrames(30)
            .WaitUntil().Show("Flame Cage C (214C)").FlameCageC().WaitFrames(30)

            .WaitUntil().Show("Banishing Rays A (22A)").BanishingRaysA().WaitFrames(30)
            .WaitUntil().Show("Banishing Rays B (22B)").BanishingRaysB().WaitFrames(30)
            .WaitUntil().Show("Planar Haze (22C)").PlanarHaze().WaitFrames(30)

            .WaitUntil().Show("Pyro: Flaming Belobog A (214214A)").PyroFlamingBelobogA().WaitFrames(30)
            .WaitUntil().Show("Pyro: Flaming Belobog B (214214B)").PyroFlamingBelobogB().WaitFrames(30)
            .WaitUntil().Show("Pyro: Flaming Belobog C (214214C)").PyroFlamingBelobogC().WaitFrames(30)
            .WaitUntil().Show("Jamming Dark (632146D)").JammingDark().WaitFrames(30)
            .WaitUntil().Show("Dreadnought Destroyer (64641236C)").DreadnoughtDestroyer().WaitFrames(30)

            .Finish(runScript: false, alsoOpenInNotepad: false);
        }

        [Test]
        public void NetaCombo2()
        {
            // Example:
            BBCF_TAS.CreateScript(@"D:\User\Downloads\Kokonoe\NetaRoute2.ahk", cfg =>
            {

            })
            .MidscreenReset() //Presses backspace key to reset state
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            ._F1() //Recording standby
            ._F1() //Recording
            .FaceLeft()
            .Run(5)
            ._214D() //Scythe
            .WaitFrames(10)
            ._F1() //Save
            .WaitFrames(10)
            .MidscreenReset() //Presses backspace key to reset state
            .FaceRight()
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            .WaitFrames(20)
            ._3D().WaitFrames(8)
            ._214A().WaitFrames(11)
            ._F2() //Play
            .WaitFrames(48)
            .Activate()
            .MoveForward(40)
            ._5A().WaitFrames(1)
            .FaceLeft()
            ._5B().WaitFrames(15)
            ._6A().WaitFrames(15)
            ._6C().WaitFrames(20)
            ._214214A().WaitFrames(100)
            ._9D().WaitFrames(30)
            ._22B().WaitFrames(9)
            ._5D().WaitFrames(20)
            ._236C().WaitFrames(50)
            .RapidCancel()
            .Overdrive().WaitFrames(80)
            ._5D().WaitFrames(30)
            .Activate().WaitFrames(30)
            .Overdrive()
            .Finish(runScript: false, alsoOpenInNotepad: false);
        }

        [Test]
        public void NetaCombo3()
        {
            // Example:
            BBCF_TAS.CreateScript(@"D:\User\Downloads\Kokonoe\NetaRoute3.ahk", cfg =>
            {

            })
            .MidscreenReset() //Presses backspace key to reset state
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            ._F1() //Recording standby
            ._F1() //Recording
            .FaceLeft()
            .Run(5)
            ._214D() //Scythe
            .WaitFrames(10)
            ._F1() //Save
            .WaitFrames(10)
            .MidscreenReset() //Presses backspace key to reset state
            .FaceRight()
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            .WaitFrames(20)
            ._3D().WaitFrames(8)
            ._214A().WaitFrames(11)
            ._F2() //Play
            .WaitFrames(48)
            .Activate()
            .MoveForward(40)
            ._5A().WaitFrames(1)
            .FaceLeft()
            ._5B().WaitFrames(15)
            ._6A().WaitFrames(15)
            ._6C().WaitFrames(20)
            .Jump().WaitFrames(20)
            ._2C().WaitFrames(20)
            .Overdrive().WaitFrames(75)
            ._2C().WaitFrames(15)
            ._214214A().WaitFrames(100)
            ._6D().WaitFrames(30)
            .JammingDark()
            //.Overdrive().WaitFrames(90)
            //._9D().WaitFrames(30)
            //.Activate()
            //.JammingDark()
            //._214214A().WaitFrames(100)
            //._9D().WaitFrames(30)
            //._22B().WaitFrames(9)
            //._5D().WaitFrames(20)
            //._236C().WaitFrames(50)
            //.RapidCancel()
            //.Overdrive().WaitFrames(80)
            //._5D().WaitFrames(30)
            //.Activate().WaitFrames(30)
            //.Overdrive()
            .Finish(runScript: false, alsoOpenInNotepad: false);


        }

        [Test]
        public void NetaCombo4()
        {
            // Example:
            BBCF_TAS.CreateScript(@"D:\User\Downloads\Kokonoe\NetaRoute4.ahk", cfg =>
            {

            })
            .MidscreenReset() //Presses backspace key to reset state
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            ._F1() //Recording standby
            ._F1() //Recording
            .FaceLeft()
            .Run(5)
            ._214D() //Scythe
            .WaitFrames(10)
            ._F1() //Save
            .WaitFrames(10)
            .MidscreenReset() //Presses backspace key to reset state
            .FaceRight()
            .WaitFrames(60)     // give the game a second to settle
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            .Jump().WaitFrames(4)
            .BackDash().WaitFrames(50)
            //This gets you to furthest corner consistently
            .WaitFrames(20)
            ._3D().WaitFrames(8)
            ._214A().WaitFrames(11)
            ._F2() //Play
            .WaitFrames(48)
            .Activate()
            .MoveForward(40)
            ._5A().WaitFrames(1)
            .FaceLeft()
            ._5B().WaitFrames(15)
            ._6A().WaitFrames(15)
            ._6C().WaitFrames(20)
            .Jump().WaitFrames(20)
            ._2C().WaitFrames(20)
            .Overdrive().WaitFrames(75)
            ._2C().WaitFrames(15)
            ._214214A().WaitFrames(100)
            ._6D().WaitFrames(30)
            .JammingDark()
            //.Overdrive().WaitFrames(90)
            //._9D().WaitFrames(30)
            //.Activate()
            //.JammingDark()
            //._214214A().WaitFrames(100)
            //._9D().WaitFrames(30)
            //._22B().WaitFrames(9)
            //._5D().WaitFrames(20)
            //._236C().WaitFrames(50)
            //.RapidCancel()
            //.Overdrive().WaitFrames(80)
            //._5D().WaitFrames(30)
            //.Activate().WaitFrames(30)
            //.Overdrive()
            .Finish(runScript: false, alsoOpenInNotepad: false);


        }
    }
}
