using System;
using System.Collections.Generic;
using System.Text;
using Portal2;
using Portal2.DataTypes;
using Portal2.Items;

namespace TestPuzzle
{
    internal static class Test
    {
        /// <summary>
        /// Creates and returns a test puzzle.
        /// </summary>
        /// <remarks>
        /// This method will test the major functionality of the library (though is by no means comprehensive).
        /// Setting all of these to true will likely produce a puzzle with too many items to compile.
        /// </remarks>
        internal static Puzzle GetTestPuzzle(
            bool voxel,
            bool angledPanel,
            bool barrier,
            bool barrierHazard,
            bool buttons,
            bool cubes,
            bool faithPlate,
            bool flipPanel,
            bool goo,
            bool laser,
            bool lightBridge,
            bool lightPanel,
            bool pistonPlatform,
            bool paint,
            bool secondaryObservationRoom,
            bool stairs,
            bool trackPlatform,
            bool tractorBeam,
            bool turret
            )
        {
            Puzzle puzzle = new Puzzle();

            puzzle.Title = "Test Puzzle";
            puzzle.Description = "A test puzzle with major functionality represented.";

            puzzle.ChamberSize = new Point3(7, 3, 3);
            puzzle.Voxels.AutoGrow = true;

            EntryChamber(puzzle);
            Hall(puzzle);
            if (voxel) VoxelFaces(puzzle);
            if (angledPanel) AngledPanel(puzzle);
            if (barrier) Barrier(puzzle);
            if (barrierHazard) BarrierHazard(puzzle);
            if (buttons) Buttons(puzzle);
            if (cubes) Cubes(puzzle);
            if (faithPlate) FaithPlate(puzzle);
            if (flipPanel) FlipPanels(puzzle);
            if (goo) Goo(puzzle);
            if (laser) Laser(puzzle);
            if (lightBridge) LightBridges(puzzle);
            if (lightPanel) LightPanels(puzzle);
            if (paint) Paints(puzzle);
            if (pistonPlatform) PistonPlatforms(puzzle);
            if (secondaryObservationRoom) SecondaryObservationRooms(puzzle);
            if (stairs) Stair(puzzle);
            if (trackPlatform) TrackPlatforms(puzzle);
            if (tractorBeam) TractorBeams(puzzle);
            if (turret) Turrets(puzzle);

            return puzzle;
        }

        private static void EntryChamber(Puzzle puzzle)
        {
            #region Entry Chamber

            // Entry
            Room(puzzle, 0, 0, 0, 3, 3, 4);
            Door(puzzle, Direction.X, 3, 1, 0);
            Door(puzzle, Direction.Y, 5, 3, 0);

            // Lobby
            Room(puzzle, 4, 0, 0, 3, 3, 3);
            puzzle.Voxels.SetVoxelData(new Point3(5, 1, 1), VoxelData.IsSolid | VoxelData.IsPortalable, VoxelData.IsSolid | VoxelData.NotPortalable);
            
            puzzle.Items.Add(new EntryDoor(puzzle)
            {
                VoxelPosition = new Point3(0, 0, 0),
                Wall = Direction.NegY
            });

            puzzle.Items.Add(new ExitDoor(puzzle)
            {
                VoxelPosition = new Point3(0, 2, 0),
                Wall = Direction.Y
            });

            puzzle.Items.Add(new ObservationRoom(puzzle)
            {
                VoxelPosition = new Point3(0, 1, 3),
                Wall = Direction.NegX
            });

            #endregion
        }

        #region Helpers

        private static void Door(Puzzle puzzle, Direction dir, int x, int y, int z)
        {
            puzzle.Voxels.SetVoxelData(new Point3(x, y, z), VoxelData.IsSolid, VoxelData.NotSolid);

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = new Point3(x, y, z),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = dir,
                Wall = Direction.NegZ
            });
        }

        private static void Door(Puzzle puzzle, Point3 point)
        {
            puzzle.Voxels.SetVoxelData(point, VoxelData.IsSolid, VoxelData.NotSolid);
        }

        private static void Room(Puzzle puzzle, int x, int y, int z, int w, int d, int h)
        {
            puzzle.Voxels.SetRange(new Point3(x, y, z), new Point3(w, d, h), VoxelData.IsSolid, VoxelData.NotSolid);
        }

        private static void Room(Puzzle puzzle, Point3 point, Point3 size)
        {
            puzzle.Voxels.SetRange(point, size, VoxelData.IsSolid, VoxelData.NotSolid);
        }

        private static void Room(Puzzle puzzle, Point3 point, int w, int d, int h)
        {
            puzzle.Voxels.SetRange(point, new Point3(w, d, h), VoxelData.IsSolid, VoxelData.NotSolid);
        }

        #endregion

        private static void Hall(Puzzle puzzle)
        {
            #region Hall

            Room(puzzle, 5, 4, 0, 8, 1, 1);
            Room(puzzle, 5, 5, 0, 1, 14, 1);
            Room(puzzle, 12, 5, 0, 1, 5, 1);
            Room(puzzle, 6, 18, 0, 20, 1, 1);
            Room(puzzle, 24, 4, 0, 1, 14, 1);
            
            #endregion
        }

        private static void VoxelFaces(Puzzle puzzle)
        {
            #region Voxel Faces

            Door(puzzle, Direction.NegY, 9, 3, 0);
            Point3 roomOffset = new Point3(8, 0, 0);

            puzzle.Voxels.SetRange(roomOffset, new Point3(3, 3, 3), VoxelData.IsSolid, VoxelData.NotSolid);

            Room(puzzle, roomOffset, 3, 3, 3);

            puzzle.Voxels.SetVoxelData(roomOffset, VoxelData.IsPortalableNegZ, VoxelData.NotPortalable);
            puzzle.Voxels.SetVoxelData(roomOffset, VoxelData.IsPortalableNegX, VoxelData.NotPortalable);
            puzzle.Voxels.SetVoxelData(roomOffset, VoxelData.IsPortalableNegY, VoxelData.NotPortalable);

            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(2, 2, 2), VoxelData.PosPortalable, VoxelData.NotPortalable);

            #endregion
        }

        private static void AngledPanel(Puzzle puzzle)
        {
            #region Angled Panel

            Point3 roomOffset = new Point3(7, 6, 0);

            Door(puzzle, Direction.Y, 9, 5, 0);
            Room(puzzle, roomOffset, 4, 4, 5);

            AngledPanel a = new AngledPanel(puzzle)
            {
                VoxelPosition = roomOffset,
                ExtendAngle = AngledPanelExtendAngle.Angle30,
                ExtendDirection = Direction.NegX,
                IsGlass = false,
                IsPortalable = false,
                StartDeployed = false,
                Wall = Direction.NegZ
            };

            AngledPanel b = new AngledPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                ExtendAngle = AngledPanelExtendAngle.Angle45,
                ExtendDirection = Direction.NegY,
                IsGlass = false,
                IsPortalable = true,
                StartDeployed = true,
                Wall = Direction.NegZ
            };

            AngledPanel c = new AngledPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 2, 0),
                ExtendAngle = AngledPanelExtendAngle.Angle60,
                ExtendDirection = Direction.X,
                IsGlass = true,
                IsPortalable = false,
                StartDeployed = false,
                Wall = Direction.NegZ
            };

            AngledPanel d = new AngledPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 3, 0),
                ExtendAngle = AngledPanelExtendAngle.Angle90,
                ExtendDirection = Direction.Y,
                IsGlass = true,
                IsPortalable = true,
                StartDeployed = true,
                Wall = Direction.NegZ
            };

            Button button = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 1, 0),
                ButtonType = ButtonType.Standard
            };

            Button button2 = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                ButtonType = ButtonType.Standard
            };

            button.ConnectionSender.ConnectTo(a.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(b.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(c.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(d.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { button, button2, a, b, c, d });

            Direction[] panelDirs = new Direction[] { Direction.NegX, Direction.Y, Direction.X, Direction.NegY };
            int[][] extendDirs = new int[][] {
                new int[] { 1, 2, 4, 5 },
                new int[] { 0, 2, 3, 5 },
                new int[] { 1, 2, 4, 5 },
                new int[] { 0, 2, 3, 5 }
            };
            Point3 location = roomOffset + Point3.UnitZ;
            Point3 direction = new Point3(0, 1, 0);
            for (int i = 0; i < panelDirs.Length; i++)
            {
                direction = ItemFacing.DirectionToPoint(ItemFacing.Cross(ItemFacing.OppositeDirection(panelDirs[i]), Direction.NegZ));
                for (int j = 0; j < extendDirs[i].Length; j++)
                {
                    AngledPanel p = new AngledPanel(puzzle)
                    {
                        VoxelPosition = location,
                        Wall = panelDirs[i],
                        ExtendDirection = (Direction)extendDirs[i][j]
                    };

                    if (i < 2)
                        button.ConnectionSender.ConnectTo(p.ConnectionReceiver);
                    else
                        button2.ConnectionSender.ConnectTo(p.ConnectionReceiver);
                    puzzle.Items.Add(p);

                    location += direction;
                }
                location -= direction;
                location += Point3.UnitZ;
            }

            #endregion
        }

        private static void Barrier(Puzzle puzzle)
        {
            #region Barrier

            Door(puzzle, Direction.X, 13, 4, 0);

            Point3 roomOffset = new Point3(14, 0, 0);
            Room(puzzle, roomOffset, 5, 5, 5);

            puzzle.Items.Add(new Barrier(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0),
                BarrierType = BarrierType.Glass,
                Facing = Direction.NegX,
                Right = Direction.Y
            });

            puzzle.Items.Add(new Barrier(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 3, 0),
                BarrierType = BarrierType.Grating,
                Facing = Direction.Y,
                Right = Direction.X,
                UpExtent = 2
            });

            puzzle.Items.Add(new Barrier(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                BarrierType = BarrierType.Glass,
                Facing = Direction.X,
                Right = Direction.NegY,
                LeftExtent = 2
            });

            puzzle.Items.Add(new Barrier(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                BarrierType = BarrierType.Grating,
                Facing = Direction.NegY,
                Right = Direction.NegX,
                RightExtent = 2
            });

            puzzle.Items.Add(new Barrier(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 3, 3),
                BarrierType = BarrierType.Grating,
                Facing = Direction.NegZ,
                Right = Direction.NegY,
                LeftExtent = 1,
                UpExtent = 1,
                RightExtent = 2
            });

            #endregion
        }

        private static void BarrierHazard(Puzzle puzzle)
        {
            #region Barrier Hazard

            Door(puzzle, Direction.X, 13, 6, 0);

            Point3 roomOffset = new Point3(14, 6, 0);
            Room(puzzle, roomOffset, 5, 5, 5);

            puzzle.Items.Add(new BarrierHazard(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                Wall = Direction.NegZ,
                HazardRight = Direction.NegX,
                HazardType = BarrierHazardType.Fizzler,
                IsOffset = false,
                NegExtent = 1,
                PosExtent = 2,
                StartEnabled = true
            });

            puzzle.Items.Add(new BarrierHazard(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 3, 3),
                Wall = Direction.NegX,
                HazardRight = Direction.NegZ,
                HazardType = BarrierHazardType.Laserfield,
                IsOffset = true,
                NegExtent = 2,
                PosExtent = 1,
                StartEnabled = true
            });

            #endregion
        }

        private static void Buttons(Puzzle puzzle)
        {
            #region Setup

            Door(puzzle, Direction.NegX, 4, 7, 0);

            Point3 roomOffset = new Point3(0, 7, 0);
            Room(puzzle, roomOffset, 4, 4, 4);

            AngledPanel ap = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(0, 3, 0),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel bp = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(1, 3, 0),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel cp = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(2, 3, 0),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel dp = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(0, 3, 1),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel ep = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(1, 3, 1),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel fp = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(2, 3, 1),
                ExtendDirection = Direction.NegZ
            };

            AngledPanel gp = new AngledPanel(puzzle)
            {
                Wall = Direction.Y,
                VoxelPosition = roomOffset + new Point3(0, 3, 2),
                ExtendDirection = Direction.NegZ
            };

            puzzle.Items.AddRange(new Item[] { ap, bp, cp, dp, ep, fp, gp });

            #endregion

            #region Button

            Button a = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 0, 0),
                ButtonType = ButtonType.Standard,
                Wall = Direction.NegZ
            };

            Button b = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                ButtonType = ButtonType.Cube,
                Wall = Direction.NegZ
            };

            Button c = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 2, 0),
                ButtonType = ButtonType.Sphere,
                Wall = Direction.NegZ
            };

            Cube cube = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 3, 0),
                DropperVisible = false,
                CubeType = CubeType.Standard
            };

            Cube sphere = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                DropperVisible = false,
                CubeType = CubeType.Sphere
            };

            puzzle.Items.AddRange(new Item[] { a, b, c, cube, sphere });

            a.ConnectionSender.ConnectTo(ap.ConnectionReceiver);
            b.ConnectionSender.ConnectTo(bp.ConnectionReceiver);
            c.ConnectionSender.ConnectTo(cp.ConnectionReceiver);

            #endregion

            #region ButtonPedestal

            ButtonPedestal pa = new ButtonPedestal(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                Wall = Direction.NegZ,
                PedestalOrientation = Direction.NegY,
                TimerDelay = 1
            };

            ButtonPedestal pb = new ButtonPedestal(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                Wall = Direction.NegZ,
                PedestalOrientation = Direction.NegX,
                TimerDelay = 2
            };

            ButtonPedestal pc = new ButtonPedestal(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0),
                Wall = Direction.NegZ,
                PedestalOrientation = Direction.Y,
                TimerDelay = 3
            };

            ButtonPedestal pd = new ButtonPedestal(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                Wall = Direction.NegZ,
                PedestalOrientation = Direction.X,
                TimerDelay = 0
            };

            puzzle.Items.AddRange(new Item[] { pa, pb, pc, pd });

            pa.ConnectionSender.ConnectTo(dp.ConnectionReceiver);
            pb.ConnectionSender.ConnectTo(ep.ConnectionReceiver);
            pc.ConnectionSender.ConnectTo(fp.ConnectionReceiver);
            pd.ConnectionSender.ConnectTo(gp.ConnectionReceiver);

            #endregion
        }

        private static void Cubes(Puzzle puzzle)
        {
            #region Cube

            Door(puzzle, Direction.NegX, 4, 12, 0);

            Point3 roomOffset = new Point3(1, 12, 0);
            Room(puzzle, roomOffset, new Point3(3, 5, 5));

            BarrierHazard fizzler = new BarrierHazard(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 4, 0),
                HazardRight = Direction.Y,
                HazardType = BarrierHazardType.Fizzler,
                StartEnabled = true,
                Wall = Direction.NegZ,
                IsOffset = true
            };

            Cube a = new Cube(puzzle)
            {
                VoxelPosition = roomOffset,
                CubeType = CubeType.Companion,
                AutoDrop = true,
                AutoRespawn = true,
                DropperVisible = true
            };

            Cube b = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                CubeType = CubeType.Franken,
                AutoDrop = false,
                AutoRespawn = true,
                DropperVisible = true
            };

            Cube c = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 2, 0),
                CubeType = CubeType.Reflective,
                AutoDrop = true,
                AutoRespawn = false,
                DropperVisible = true
            };

            Cube d = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 3, 0),
                CubeType = CubeType.Sphere,
                AutoDrop = true,
                AutoRespawn = true,
                DropperVisible = false
            };

            Cube e = new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 4, 0),
                CubeType = CubeType.Standard,
                AutoDrop = true,
                AutoRespawn = true,
                DropperVisible = true
            };

            Button button = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 3, 0)
            };

            button.ConnectionSender.ConnectTo(b.Dropper.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(c.Dropper.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { fizzler, a, b, c, d, e, button });

            #endregion
        }

        private static void FaithPlate(Puzzle puzzle)
        {
            #region Faith Plate

            Door(puzzle, Direction.X, 6, 11, 0);
            Door(puzzle, Direction.Y, 12, 10, 0);

            Point3 roomOffset = new Point3(7, 11, 0);
            Room(puzzle, roomOffset, 6, 6, 6);

            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(1, 0, 0), VoxelData.AllPortalable, VoxelData.NotPortalable);

            FaithPlate o = CreateFaithPlate(puzzle, roomOffset, 3, 5, 3, Direction.Y, 1.0, null);
            o.Target.VoxelPosition = roomOffset + new Point3(4, 4, 0);
            o.Target.Wall = Direction.NegZ;

            FaithPlate n = CreateFaithPlate(puzzle, roomOffset, 3, 4, 0, Direction.NegZ, 3.0, o);
            FaithPlate m = CreateFaithPlate(puzzle, roomOffset, 0, 3, 2, Direction.NegX, 2.0, n);
            FaithPlate l = CreateFaithPlate(puzzle, roomOffset, 5, 3, 2, Direction.X, 0.5, m);
            FaithPlate k = CreateFaithPlate(puzzle, roomOffset, 4, 5, 1, Direction.Y, 3.0, l);
            FaithPlate j = CreateFaithPlate(puzzle, roomOffset, 4, 0, 1, Direction.NegY, 2.0, k);
            FaithPlate i = CreateFaithPlate(puzzle, roomOffset, 5, 1, 0, Direction.NegZ, 4.0, j);
            FaithPlate h = CreateFaithPlate(puzzle, roomOffset, 3, 5, 4, Direction.Y, 1.0, i);
            FaithPlate g = CreateFaithPlate(puzzle, roomOffset, 0, 2, 0, Direction.NegX, 4.0, h);
            FaithPlate f = CreateFaithPlate(puzzle, roomOffset, 5, 4, 0, Direction.X, 2.0, g);
            FaithPlate e = CreateFaithPlate(puzzle, roomOffset, 1, 3, 0, Direction.NegZ, 5.0, f);
            FaithPlate d = CreateFaithPlate(puzzle, roomOffset, 2, 4, 0, Direction.NegZ, 3.0, e);
            FaithPlate c = CreateFaithPlate(puzzle, roomOffset, 3, 0, 0, Direction.NegZ, 4.0, d);
            FaithPlate b = CreateFaithPlate(puzzle, roomOffset, 1, 5, 0, Direction.NegZ, 2.0, c);
            FaithPlate a = CreateFaithPlate(puzzle, roomOffset, 1, 0, 0, Direction.NegZ, 0.5, b);

            #endregion
        }

        #region Faith Plate Helper

        private static FaithPlate CreateFaithPlate(Puzzle puzzle, Point3 roomOffset,
            int x, int y, int z, Direction wall, double arcHeight, FaithPlate target)
        {
            FaithPlate plate = new FaithPlate(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(x, y, z),
                Wall = wall,
                ArcHeight = arcHeight
            };

            puzzle.Items.Add(plate);

            if (target != null)
            {
                plate.Target.VoxelPosition = target.VoxelPosition;
                plate.Target.Wall = target.Wall;
            }

            return plate;
        }

        #endregion

        private static void FlipPanels(Puzzle puzzle)
        {
            #region FlipPanel

            Door(puzzle, Direction.NegX, 4, 18, 0);

            Point3 roomOffset = new Point3(0, 18, 0);
            Room(puzzle, roomOffset, 4, 4, 4);

            FlipPanel a = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 1, 0),
                FlipAxis = Direction.X,
                Wall = Direction.NegZ
            };
            FlipPanel b = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0),
                FlipAxis = Direction.Y,
                Wall = Direction.NegZ,
                StartFlipped = true
            };
            FlipPanel c = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                FlipAxis = Direction.NegX,
                Wall = Direction.NegZ,
                IsPortalable = false
            };
            FlipPanel d = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 2, 0),
                FlipAxis = Direction.NegY,
                Wall = Direction.NegZ,
                IsPortalable = false,
                StartFlipped = true
            };

            FlipPanel e = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 1),
                FlipAxis = Direction.Y,
                Wall = Direction.NegX
            };

            FlipPanel f = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 3, 0),
                FlipAxis = Direction.X,
                Wall = Direction.Y
            };


            FlipPanel g = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                FlipAxis = Direction.NegZ,
                Wall = Direction.X
            };


            FlipPanel h = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                FlipAxis = Direction.Z,
                Wall = Direction.NegY
            };

            Button button = new Button(puzzle)
            {
                VoxelPosition = roomOffset
            };

            button.ConnectionSender.ConnectTo(a.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(b.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(c.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(d.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(e.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(f.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(g.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(h.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { a, b, c, d, e, f, g, h, button });

            #endregion
        }

        private static void Goo(Puzzle puzzle)
        {
            #region Goo

            Door(puzzle, Direction.Y, 5, 19, 0);

            Point3 roomOffset = new Point3(5, 20, 0);
            Room(puzzle, roomOffset, 5, 4, 5);

            // Create room dimensions
            puzzle.Voxels.SetRange(roomOffset + new Point3(0, 1, 0), new Point3(5, 3, 3), VoxelData.IsSolid, VoxelData.IsSolid);
            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(1, 2, 2), VoxelData.IsSolid, VoxelData.NotSolid);
            puzzle.Voxels.SetRange(roomOffset + new Point3(3, 2, 1), new Point3(2, 2, 2), VoxelData.IsSolid, VoxelData.NotSolid);
            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(3, 2, 1), VoxelData.IsSolid, VoxelData.IsSolid);
            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(4, 3, 2), VoxelData.IsSolid, VoxelData.IsSolid);

            puzzle.Voxels.SetVoxelData(roomOffset + new Point3(4, 0, 2), VoxelData.IsSolid, VoxelData.IsSolid);

            // Add launcher
            FaithPlate plate = new FaithPlate(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                ArcHeight = 4.0
            };
            plate.Target.VoxelPosition = roomOffset + new Point3(4, 0, 3);
            plate.Target.Wall = Direction.NegZ;
            puzzle.Items.Add(plate);

            // Add goo items
            puzzle.Items.Add(new Goo(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 2)
            });

            puzzle.Items.Add(new Goo(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 2)
            });

            #endregion
        }

        private static void Laser(Puzzle puzzle)
        {
            #region Laser Emitter

            Door(puzzle, Direction.Y, 11, 19, 0);

            Point3 roomOffset = new Point3(11, 20, 0);
            Room(puzzle, roomOffset, 4, 4, 3);

            puzzle.Items.Add(new LaserEmitter(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                IsCentered = true,
                StartEnabled = true,
                Wall = Direction.NegZ
            });

            LaserEmitter ea = new LaserEmitter(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                Wall = Direction.NegY,
                StartEnabled = false,
                IsCentered = false,
                OffsetDirection = Direction.NegZ
            };

            puzzle.Items.Add(ea);

            #endregion

            #region Laser Catcher

            puzzle.Items.Add(new Cube(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 3, 0),
                CubeType = CubeType.Reflective
            });

            FlipPanel pa = new FlipPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 3, 0),
                Wall = Direction.Y,
                FlipAxis = Direction.Z
            };

            puzzle.Items.Add(pa);

            LaserCatcher ca = new LaserCatcher(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 3, 0),
                Wall = Direction.NegZ,
                IsCentered = true
            };

            puzzle.Items.Add(ca);

            ca.ConnectionSender.ConnectTo(ea.ConnectionReceiver);

            LaserCatcher cb = new LaserCatcher(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                Wall = Direction.X,
                IsCentered = false,
                OffsetDirection = Direction.NegZ
            };

            puzzle.Items.Add(cb);
            cb.ConnectionSender.ConnectTo(pa.ConnectionReceiver);

            #endregion

            #region Laser Relay

            LaserRelay ra = new LaserRelay(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 1, 0),
                Wall = Direction.NegZ,
                IsCentered = false,
                OffsetDirection = Direction.NegY
            };

            puzzle.Items.Add(ra);
            ra.ConnectionSender.ConnectTo(pa.ConnectionReceiver);

            LaserRelay rb = new LaserRelay(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 2, 0),
                IsCentered = true,
                Wall = Direction.NegZ
            };

            puzzle.Items.Add(rb);
            rb.ConnectionSender.ConnectTo(pa.ConnectionReceiver);

            #endregion
        }

        private static void LightBridges(Puzzle puzzle)
        {
            #region Light Bridge

            Door(puzzle, Direction.NegY, 14, 17, 0);

            Point3 roomOffset = new Point3(14, 12, 0);
            Room(puzzle, roomOffset, 3, 5, 5);

            puzzle.Items.Add(new LightBridge(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 0, 1),
                Wall = Direction.NegX,
                BridgeTop = Direction.Z,
            });

            Button b = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 3, 0)
            };

            LightBridge a = new LightBridge(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0),
                BridgeTop = Direction.Y,
                Wall = Direction.NegZ,
                StartEnabled = false
            };

            puzzle.Items.AddRange(new Item[] { a, b });

            b.ConnectionSender.ConnectTo(a.ConnectionReceiver);

            #endregion
        }

        private static void LightPanels(Puzzle puzzle)
        {
            #region Light Panels

            Door(puzzle, Direction.Y, 16, 19, 0);

            Point3 roomOffset = new Point3(16, 20, 0);
            Room(puzzle, roomOffset, 4, 4, 3);

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 0, 0),
                OffsetDirection = Direction.NegX,
                OffsetAmount = LightOffsetAmount.Far
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                OffsetDirection = Direction.NegX,
                OffsetAmount = LightOffsetAmount.Near
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                OffsetDirection = Direction.X,
                OffsetAmount = LightOffsetAmount.Near
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 1, 0),
                OffsetDirection = Direction.X,
                OffsetAmount = LightOffsetAmount.Far
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 2, 0),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.NegY,
                Wall = Direction.NegZ
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 3, 0),
                Wall = Direction.NegZ,
                OffsetDirection = Direction.NegY,
                OffsetAmount = LightOffsetAmount.Near
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                OffsetAmount = LightOffsetAmount.Near,
                OffsetDirection = Direction.Y,
                Wall = Direction.NegZ
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 3, 0),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.Y,
                Wall = Direction.NegZ,
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 2, 1),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.NegY,
                Wall = Direction.NegX
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 3, 1),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.NegZ,
                Wall = Direction.Y
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 1),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.Z,
                Wall = Direction.X
            });

            puzzle.Items.Add(new LightPanel(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 1),
                OffsetAmount = LightOffsetAmount.Far,
                OffsetDirection = Direction.NegX,
                Wall = Direction.NegY
            });

            #endregion
        }

        private static void Paints(Puzzle puzzle)
        {
            #region Paint

            Door(puzzle, Direction.NegY, 18, 17, 0);

            Point3 roomOffset = new Point3(18, 12, 0);
            Room(puzzle, roomOffset, 5, 5, 5);

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset,
                PaintType = PaintType.Bounce,
                Wall = Direction.NegZ,
                FlowType = PaintFlowType.Light
            });

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                PaintType = PaintType.Cleansing,
                Wall = Direction.NegZ,
                FlowType = PaintFlowType.Medium
            });

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                PaintType = PaintType.Conversion,
                Wall = Direction.NegZ,
                FlowType = PaintFlowType.Heavy
            });

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(3, 0, 0),
                PaintType = PaintType.Speed,
                Wall = Direction.NegZ,
                FlowType = PaintFlowType.Drip
            });

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(4, 0, 0),
                PaintType = PaintType.Bounce,
                Wall = Direction.NegZ,
                FlowType = PaintFlowType.Bomb
            });

            Paint a = new Paint(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                Wall = Direction.NegZ,
                AllowStreaks = true
            };
            a.Dropper.VoxelPosition = roomOffset + new Point3(0, 1, 2);
            a.Dropper.Wall = Direction.NegX;
            puzzle.Items.Add(a);

            Paint b = new Paint(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 2, 0),
                Wall = Direction.NegZ,
                AllowStreaks = false
            };
            b.Dropper.VoxelPosition = roomOffset + new Point3(0, 2, 2);
            b.Dropper.Wall = Direction.NegX;
            puzzle.Items.Add(b);

            Paint c = new Paint(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(4, 3, 0),
                Wall = Direction.X,
                AllowStreaks = true
            };
            c.Dropper.VoxelPosition = roomOffset + new Point3(0, 3, 3);
            c.Dropper.Wall = Direction.NegX;
            puzzle.Items.Add(c);

            Paint d = new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(2, 4, 0),
                Wall = Direction.NegZ,
                AllowStreaks = false,
                StartEnabled = false
            };
            puzzle.Items.Add(d);

            Button button = new Button(puzzle) { VoxelPosition = roomOffset + new Point3(1, 4, 0) };
            button.ConnectionSender.ConnectTo(d.Dropper.ConnectionReceiver);
            puzzle.Items.Add(button);

            puzzle.Items.Add(new Paint(puzzle)
            {
                AutoPosition = true,
                VoxelPosition = roomOffset + new Point3(4, 4, 0),
                Wall = Direction.NegZ,
                AllowStreaks = false,
                DropperVisible = false
            });

            #endregion
        }

        private static void PistonPlatforms(Puzzle puzzle)
        {
            #region PistonPlatform

            Door(puzzle, Direction.X, 26, 18, 0);

            Point3 roomOffset = new Point3(27, 16, 0);
            Room(puzzle, roomOffset, 3, 3, 5);

            puzzle.Items.Add(new PistonPlatform(puzzle)
            {
                VoxelPosition = roomOffset,
                Wall = Direction.NegZ,
                NearExtent = 1,
                FarExtent = 3
            });

            puzzle.Items.Add(new PistonPlatform(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 1),
                Wall = Direction.NegY,
                NearExtent = 0,
                FarExtent = 2
            });

            PistonPlatform a = new PistonPlatform(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                Wall = Direction.NegZ,
                NearExtent = 0,
                FarExtent = 4
            };
            puzzle.Items.Add(a);

            Button button = new Button(puzzle) { VoxelPosition = roomOffset + new Point3(2, 1, 0) };
            puzzle.Items.Add(button);

            button.ConnectionSender.ConnectTo(a.ConnectionReceiver);

            #endregion
        }

        private static void SecondaryObservationRooms(Puzzle puzzle)
        {
            #region Secondary Observation Room

            Door(puzzle, Direction.Y, 22, 19, 0);

            Point3 roomOffset = new Point3(21, 20, 0);
            Room(puzzle, roomOffset, new Point3(3, 3, 5));

            puzzle.Items.Add(new SecondaryObservationRoom(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 4),
                Wall = Direction.NegY
            });

            puzzle.Items.Add(new SecondaryObservationRoom(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 4),
                Wall = Direction.NegX
            });

            puzzle.Items.Add(new SecondaryObservationRoom(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 4),
                Wall = Direction.Y
            });

            puzzle.Items.Add(new SecondaryObservationRoom(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 4),
                Wall = Direction.X
            });

            #endregion
        }

        private static void Stair(Puzzle puzzle)
        {
            #region Stairs

            Door(puzzle, Direction.X, 25, 12, 0);

            Point3 roomOffset = new Point3(26, 10, 0);
            Room(puzzle, roomOffset, 4, 4, 3);

            Stairs a = new Stairs(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                StairDirection = Direction.Y,
                StartDeployed = false
            };

            Stairs b = new Stairs(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 3, 0),
                StairDirection = Direction.X,
                StartDeployed = true
            };

            Stairs c = new Stairs(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(3, 2, 0),
                StairDirection = Direction.NegY,
                StartDeployed = true
            };

            Stairs d = new Stairs(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                StairDirection = Direction.NegX,
                StartDeployed = false
            };

            Stairs e = new Stairs(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 2),
                StairDirection = Direction.Y,
                Wall = Direction.NegX,
                StartDeployed = false
            };

            Button button = new Button(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0)
            };

            button.ConnectionSender.ConnectTo(a.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(c.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(d.ConnectionReceiver);
            button.ConnectionSender.ConnectTo(e.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { a, b, c, d, e, button });

            #endregion
        }

        private static void TrackPlatforms(Puzzle puzzle)
        {
            #region TrackPlatform

            Door(puzzle, Direction.NegX, 23, 10, 0);

            Point3 roomOffset = new Point3(20, 7, 0);
            Room(puzzle, roomOffset, 3, 4, 4);

            TrackPlatform a = new TrackPlatform(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 1),
                Wall = Direction.NegX,
                StartActive = true,
                RailOscillate = true,
                FacingDirection = Direction.Z,
                LeftExtent = 1,
                RightExtent = 2
            };

            TrackPlatform b = new TrackPlatform(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 2),
                Wall = Direction.NegY,
                StartActive = false,
                RailOscillate = false,
                FacingDirection = Direction.Z,
                BackExtent = 2,
                ForwardExtent = 1
            };

            Button button = new Button(puzzle) { VoxelPosition = roomOffset + new Point3(1, 1, 0) };
            button.ConnectionSender.ConnectTo(b.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { a, b, button });

            #endregion
        }

        private static void TractorBeams(Puzzle puzzle)
        {
            #region TractorBeam

            Door(puzzle, Direction.X, 25, 8, 0);

            Point3 roomOffset = new Point3(26, 6, 0);
            Room(puzzle, roomOffset, 3, 3, 3);

            TractorBeam a = new TractorBeam(puzzle)
            {
                VoxelPosition = roomOffset,
                Wall = Direction.NegZ
            };

            TractorBeam b = new TractorBeam(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 1),
                Wall = Direction.NegY,
                StartReversed = true,
                StartEnabled = false
            };

            Button button = new Button(puzzle) { VoxelPosition = roomOffset + new Point3(1, 2, 0) };

            button.ConnectionSender.ConnectTo(a.PolarityConnectionReceiver);
            button.ConnectionSender.ConnectTo(b.ConnectionReceiver);

            puzzle.Items.AddRange(new Item[] { a, b, button });

            #endregion
        }

        private static void Turrets(Puzzle puzzle)
        {
            #region Turret

            Door(puzzle, Direction.NegX, 23, 5, 0);

            Point3 roomOffset = new Point3(20, 3, 0);
            Room(puzzle, roomOffset, 3, 3, 3);

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 1, 0),
                FacingAngle = 0
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 0, 0),
                FacingAngle = -45
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 0, 0),
                FacingAngle = -90
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 0, 0),
                FacingAngle = -135
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 1, 0),
                FacingAngle = 180
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(0, 2, 0),
                FacingAngle = 135
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(1, 2, 0),
                FacingAngle = 90
            });

            puzzle.Items.Add(new Turret(puzzle)
            {
                VoxelPosition = roomOffset + new Point3(2, 2, 0),
                FacingAngle = 45
            });

            #endregion
        }
    }
}
