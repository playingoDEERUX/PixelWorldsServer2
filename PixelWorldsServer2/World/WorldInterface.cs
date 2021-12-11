using Kernys.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.World
{
    public interface WorldInterface
    {
		public enum BlockClass
		{
			// Token: 0x0400144C RID: 5196
			GroundGeneric,
			// Token: 0x0400144D RID: 5197
			GroundSoft,
			// Token: 0x0400144E RID: 5198
			GroundHard,
			// Token: 0x0400144F RID: 5199
			GroundVegetation,
			// Token: 0x04001450 RID: 5200
			GroundIndestructible,
			// Token: 0x04001451 RID: 5201
			WeaponGeneric,
			// Token: 0x04001452 RID: 5202
			WeaponBlade,
			// Token: 0x04001453 RID: 5203
			WeaponPickaxe,
			// Token: 0x04001454 RID: 5204
			WeaponGun
		}

		// Token: 0x02000233 RID: 563
		public enum BlockMaterialClass
		{
			// Token: 0x04001456 RID: 5206
			Generic,
			// Token: 0x04001457 RID: 5207
			Metal,
			// Token: 0x04001458 RID: 5208
			Stone,
			// Token: 0x04001459 RID: 5209
			Wood,
			// Token: 0x0400145A RID: 5210
			Ground,
			// Token: 0x0400145B RID: 5211
			Tree,
			// Token: 0x0400145C RID: 5212
			Indestructible,
			// Token: 0x0400145D RID: 5213
			Cow,
			// Token: 0x0400145E RID: 5214
			Sheep,
			// Token: 0x0400145F RID: 5215
			Chicken,
			// Token: 0x04001460 RID: 5216
			HardStone,
			// Token: 0x04001461 RID: 5217
			EventItemStPatricks,
			// Token: 0x04001462 RID: 5218
			Gooey,
			// Token: 0x04001463 RID: 5219
			EventItemSummer,
			// Token: 0x04001464 RID: 5220
			END_OF_ENUM
		}

		// Token: 0x02000234 RID: 564
		public enum BlockType
		{
			// Token: 0x04001466 RID: 5222
			None,
			// Token: 0x04001467 RID: 5223
			SoilBlock,
			// Token: 0x04001468 RID: 5224
			CaveWall,
			// Token: 0x04001469 RID: 5225
			Bedrock,
			// Token: 0x0400146A RID: 5226
			Granite,
			// Token: 0x0400146B RID: 5227
			Sand,
			// Token: 0x0400146C RID: 5228
			SandStone,
			// Token: 0x0400146D RID: 5229
			Lava,
			// Token: 0x0400146E RID: 5230
			Marble,
			// Token: 0x0400146F RID: 5231
			Obsidian,
			// Token: 0x04001470 RID: 5232
			Grass,
			// Token: 0x04001471 RID: 5233
			Rose,
			// Token: 0x04001472 RID: 5234
			Sunflower,
			// Token: 0x04001473 RID: 5235
			Lily,
			// Token: 0x04001474 RID: 5236
			Blueberry,
			// Token: 0x04001475 RID: 5237
			MetalPlate,
			// Token: 0x04001476 RID: 5238
			WoodenPlatform,
			// Token: 0x04001477 RID: 5239
			Ice,
			// Token: 0x04001478 RID: 5240
			Water,
			// Token: 0x04001479 RID: 5241
			GreyBrick,
			// Token: 0x0400147A RID: 5242
			RedBrick,
			// Token: 0x0400147B RID: 5243
			YellowBrick,
			// Token: 0x0400147C RID: 5244
			WhiteBrick,
			// Token: 0x0400147D RID: 5245
			BlackBrick,
			// Token: 0x0400147E RID: 5246
			WoodWall,
			// Token: 0x0400147F RID: 5247
			WoodBlock,
			// Token: 0x04001480 RID: 5248
			WoodenBackground,
			// Token: 0x04001481 RID: 5249
			JungleGrass,
			// Token: 0x04001482 RID: 5250
			MetalPlatform,
			// Token: 0x04001483 RID: 5251
			Microwave,
			// Token: 0x04001484 RID: 5252
			WoodenTable,
			// Token: 0x04001485 RID: 5253
			Brazier,
			// Token: 0x04001486 RID: 5254
			WindowFrame,
			// Token: 0x04001487 RID: 5255
			GreenJello,
			// Token: 0x04001488 RID: 5256
			YellowJello,
			// Token: 0x04001489 RID: 5257
			BlueJello,
			// Token: 0x0400148A RID: 5258
			RedJello,
			// Token: 0x0400148B RID: 5259
			Tree,
			// Token: 0x0400148C RID: 5260
			BasicFace,
			// Token: 0x0400148D RID: 5261
			BasicEyebrows,
			// Token: 0x0400148E RID: 5262
			BasicEyeballs,
			// Token: 0x0400148F RID: 5263
			BasicPupil,
			// Token: 0x04001490 RID: 5264
			BasicMouth,
			// Token: 0x04001491 RID: 5265
			BasicLegs,
			// Token: 0x04001492 RID: 5266
			BasicTorso,
			// Token: 0x04001493 RID: 5267
			BasicTopArm,
			// Token: 0x04001494 RID: 5268
			BasicBottomArm,
			// Token: 0x04001495 RID: 5269
			BasicEyelashes,
			// Token: 0x04001496 RID: 5270
			PantsBatman,
			// Token: 0x04001497 RID: 5271
			ShoesBatman,
			// Token: 0x04001498 RID: 5272
			WeaponClaymore,
			// Token: 0x04001499 RID: 5273
			WeaponExecutionerAxe,
			// Token: 0x0400149A RID: 5274
			WeaponKatana,
			// Token: 0x0400149B RID: 5275
			WeaponPickAxe,
			// Token: 0x0400149C RID: 5276
			WeaponPitchFork,
			// Token: 0x0400149D RID: 5277
			WeaponShortSword,
			// Token: 0x0400149E RID: 5278
			WeaponSpartanSword,
			// Token: 0x0400149F RID: 5279
			WeaponWalkingCane,
			// Token: 0x040014A0 RID: 5280
			WeaponGunBeretta,
			// Token: 0x040014A1 RID: 5281
			SandyCaveWall,
			// Token: 0x040014A2 RID: 5282
			LunarSoil,
			// Token: 0x040014A3 RID: 5283
			MoonRock,
			// Token: 0x040014A4 RID: 5284
			MoonBackground,
			// Token: 0x040014A5 RID: 5285
			MartianSoil,
			// Token: 0x040014A6 RID: 5286
			MartianRock,
			// Token: 0x040014A7 RID: 5287
			MartianBackground,
			// Token: 0x040014A8 RID: 5288
			MagicStuff,
			// Token: 0x040014A9 RID: 5289
			QuickSand,
			// Token: 0x040014AA RID: 5290
			RedWallpaper,
			// Token: 0x040014AB RID: 5291
			RedBlock,
			// Token: 0x040014AC RID: 5292
			YellowBlock,
			// Token: 0x040014AD RID: 5293
			BlueBlock,
			// Token: 0x040014AE RID: 5294
			WhiteBlock,
			// Token: 0x040014AF RID: 5295
			BlackBlock,
			// Token: 0x040014B0 RID: 5296
			Sign,
			// Token: 0x040014B1 RID: 5297
			Mushroom,
			// Token: 0x040014B2 RID: 5298
			Door,
			// Token: 0x040014B3 RID: 5299
			StoneBackground,
			// Token: 0x040014B4 RID: 5300
			WoodPanel,
			// Token: 0x040014B5 RID: 5301
			LavaLamp,
			// Token: 0x040014B6 RID: 5302
			SmallChest,
			// Token: 0x040014B7 RID: 5303
			GreenBlock,
			// Token: 0x040014B8 RID: 5304
			PurpleBlock,
			// Token: 0x040014B9 RID: 5305
			OrangeBlock,
			// Token: 0x040014BA RID: 5306
			LightBlueBlock,
			// Token: 0x040014BB RID: 5307
			GreyBlock,
			// Token: 0x040014BC RID: 5308
			PinkBlock,
			// Token: 0x040014BD RID: 5309
			FlowerWallpaper,
			// Token: 0x040014BE RID: 5310
			WoodenChair,
			// Token: 0x040014BF RID: 5311
			Pineapple,
			// Token: 0x040014C0 RID: 5312
			Corn,
			// Token: 0x040014C1 RID: 5313
			Lantern,
			// Token: 0x040014C2 RID: 5314
			ClassicPainting,
			// Token: 0x040014C3 RID: 5315
			RubberDuck,
			// Token: 0x040014C4 RID: 5316
			IronBlock,
			// Token: 0x040014C5 RID: 5317
			ClearJello,
			// Token: 0x040014C6 RID: 5318
			Fireplace,
			// Token: 0x040014C7 RID: 5319
			ClayPot,
			// Token: 0x040014C8 RID: 5320
			Armchair,
			// Token: 0x040014C9 RID: 5321
			GlassDoor,
			// Token: 0x040014CA RID: 5322
			FloorLamp,
			// Token: 0x040014CB RID: 5323
			FruitTray,
			// Token: 0x040014CC RID: 5324
			BrownBlock,
			// Token: 0x040014CD RID: 5325
			Strawberry,
			// Token: 0x040014CE RID: 5326
			Shoji,
			// Token: 0x040014CF RID: 5327
			GardenGnome,
			// Token: 0x040014D0 RID: 5328
			Oven,
			// Token: 0x040014D1 RID: 5329
			Cabinet,
			// Token: 0x040014D2 RID: 5330
			OldTV,
			// Token: 0x040014D3 RID: 5331
			SpikeTrap,
			// Token: 0x040014D4 RID: 5332
			EntrancePortal,
			// Token: 0x040014D5 RID: 5333
			FireTrap,
			// Token: 0x040014D6 RID: 5334
			LEDSign,
			// Token: 0x040014D7 RID: 5335
			DungeonWall,
			// Token: 0x040014D8 RID: 5336
			DungeonDoor,
			// Token: 0x040014D9 RID: 5337
			RedBrickWallpaper,
			// Token: 0x040014DA RID: 5338
			BrownBrickWallpaper,
			// Token: 0x040014DB RID: 5339
			YellowBrickWallpaper,
			// Token: 0x040014DC RID: 5340
			ClownWallpaper,
			// Token: 0x040014DD RID: 5341
			IllusorySoilBlock,
			// Token: 0x040014DE RID: 5342
			HugeMetalFan,
			// Token: 0x040014DF RID: 5343
			Shrubbery,
			// Token: 0x040014E0 RID: 5344
			Tapestry,
			// Token: 0x040014E1 RID: 5345
			CastleWallpaper,
			// Token: 0x040014E2 RID: 5346
			CastleWall,
			// Token: 0x040014E3 RID: 5347
			CastleDoor,
			// Token: 0x040014E4 RID: 5348
			IronChandelier,
			// Token: 0x040014E5 RID: 5349
			Throne,
			// Token: 0x040014E6 RID: 5350
			GreekColumn,
			// Token: 0x040014E7 RID: 5351
			Fountain,
			// Token: 0x040014E8 RID: 5352
			Torch,
			// Token: 0x040014E9 RID: 5353
			TileWhite,
			// Token: 0x040014EA RID: 5354
			Stereos,
			// Token: 0x040014EB RID: 5355
			ExtraSpeaker,
			// Token: 0x040014EC RID: 5356
			HotTub,
			// Token: 0x040014ED RID: 5357
			Safe,
			// Token: 0x040014EE RID: 5358
			ATM,
			// Token: 0x040014EF RID: 5359
			CubistPainting,
			// Token: 0x040014F0 RID: 5360
			ModernPainting,
			// Token: 0x040014F1 RID: 5361
			ImperialWallpaper,
			// Token: 0x040014F2 RID: 5362
			GoldenToilet,
			// Token: 0x040014F3 RID: 5363
			ModernSculpture,
			// Token: 0x040014F4 RID: 5364
			GoldBlock,
			// Token: 0x040014F5 RID: 5365
			BlackLeatherChair,
			// Token: 0x040014F6 RID: 5366
			SellOMat,
			// Token: 0x040014F7 RID: 5367
			LightBlueWallpaper,
			// Token: 0x040014F8 RID: 5368
			Hummer,
			// Token: 0x040014F9 RID: 5369
			Sandbag,
			// Token: 0x040014FA RID: 5370
			ArmyTent,
			// Token: 0x040014FB RID: 5371
			PlywoodWallpaper,
			// Token: 0x040014FC RID: 5372
			BarbedWire,
			// Token: 0x040014FD RID: 5373
			Tracktor,
			// Token: 0x040014FE RID: 5374
			Wheat,
			// Token: 0x040014FF RID: 5375
			Scarecrow,
			// Token: 0x04001500 RID: 5376
			BarnWall,
			// Token: 0x04001501 RID: 5377
			BarnDoor,
			// Token: 0x04001502 RID: 5378
			PicketFence,
			// Token: 0x04001503 RID: 5379
			Hayblock,
			// Token: 0x04001504 RID: 5380
			ShirtBatman,
			// Token: 0x04001505 RID: 5381
			MaskBatman,
			// Token: 0x04001506 RID: 5382
			CapeBatman,
			// Token: 0x04001507 RID: 5383
			PantsSweat,
			// Token: 0x04001508 RID: 5384
			ShoesBrown,
			// Token: 0x04001509 RID: 5385
			ShoesRubberBootsYellow,
			// Token: 0x0400150A RID: 5386
			GlassesBasic,
			// Token: 0x0400150B RID: 5387
			GlassesNerdy,
			// Token: 0x0400150C RID: 5388
			GlassesRed,
			// Token: 0x0400150D RID: 5389
			GlassesWhite,
			// Token: 0x0400150E RID: 5390
			GlassesSunBasic,
			// Token: 0x0400150F RID: 5391
			BeardBasic,
			// Token: 0x04001510 RID: 5392
			BeardGoatee,
			// Token: 0x04001511 RID: 5393
			BeardLong,
			// Token: 0x04001512 RID: 5394
			BeardBlack,
			// Token: 0x04001513 RID: 5395
			HatAcademic,
			// Token: 0x04001514 RID: 5396
			HatCapBlue,
			// Token: 0x04001515 RID: 5397
			HatCapGreen,
			// Token: 0x04001516 RID: 5398
			HatCapPink,
			// Token: 0x04001517 RID: 5399
			HatCapRed,
			// Token: 0x04001518 RID: 5400
			HatCapWhite,
			// Token: 0x04001519 RID: 5401
			MaskExecutioner,
			// Token: 0x0400151A RID: 5402
			HatFireFighter,
			// Token: 0x0400151B RID: 5403
			HatGolfBeret,
			// Token: 0x0400151C RID: 5404
			MaskMedievalKnight,
			// Token: 0x0400151D RID: 5405
			HatNavy,
			// Token: 0x0400151E RID: 5406
			HatNurse,
			// Token: 0x0400151F RID: 5407
			MaskPaperBagBrown,
			// Token: 0x04001520 RID: 5408
			HatPolice,
			// Token: 0x04001521 RID: 5409
			MaskSkimask,
			// Token: 0x04001522 RID: 5410
			HatSombrero,
			// Token: 0x04001523 RID: 5411
			HatStetson,
			// Token: 0x04001524 RID: 5412
			HatStrawHat,
			// Token: 0x04001525 RID: 5413
			HatTennisHeadband,
			// Token: 0x04001526 RID: 5414
			HatTopHat,
			// Token: 0x04001527 RID: 5415
			HatWoolcapGreen,
			// Token: 0x04001528 RID: 5416
			HairBeadedBlack,
			// Token: 0x04001529 RID: 5417
			HairBradedBlack,
			// Token: 0x0400152A RID: 5418
			HairLongBlonde,
			// Token: 0x0400152B RID: 5419
			HairLongBrown,
			// Token: 0x0400152C RID: 5420
			HairLongGolden,
			// Token: 0x0400152D RID: 5421
			HairLongPink,
			// Token: 0x0400152E RID: 5422
			HairLongCurlyBrown,
			// Token: 0x0400152F RID: 5423
			HairPigtailRed,
			// Token: 0x04001530 RID: 5424
			HairPunkBlue,
			// Token: 0x04001531 RID: 5425
			HairAfroDark,
			// Token: 0x04001532 RID: 5426
			HairArchyGrey,
			// Token: 0x04001533 RID: 5427
			HairBuzzCutBrown,
			// Token: 0x04001534 RID: 5428
			HairCasualBrown,
			// Token: 0x04001535 RID: 5429
			HairClownRed,
			// Token: 0x04001536 RID: 5430
			HairPonytailBrown,
			// Token: 0x04001537 RID: 5431
			HairSideyBlack,
			// Token: 0x04001538 RID: 5432
			HairSpikyBlack,
			// Token: 0x04001539 RID: 5433
			HairSpikyBrown,
			// Token: 0x0400153A RID: 5434
			ShirtTBlack,
			// Token: 0x0400153B RID: 5435
			ShirtTSkullRed,
			// Token: 0x0400153C RID: 5436
			CapeSparta,
			// Token: 0x0400153D RID: 5437
			WingsDemon,
			// Token: 0x0400153E RID: 5438
			WeaponGunRevolver,
			// Token: 0x0400153F RID: 5439
			WeaponGunAK47,
			// Token: 0x04001540 RID: 5440
			Chicken,
			// Token: 0x04001541 RID: 5441
			Cow,
			// Token: 0x04001542 RID: 5442
			Sheep,
			// Token: 0x04001543 RID: 5443
			ScifiPanel1,
			// Token: 0x04001544 RID: 5444
			ScifiPanel2,
			// Token: 0x04001545 RID: 5445
			ScifiPanel3,
			// Token: 0x04001546 RID: 5446
			ScifiBackground1,
			// Token: 0x04001547 RID: 5447
			ScifiBackground2,
			// Token: 0x04001548 RID: 5448
			ScifiLights,
			// Token: 0x04001549 RID: 5449
			ScifiDoor,
			// Token: 0x0400154A RID: 5450
			ScifiGenerator,
			// Token: 0x0400154B RID: 5451
			ScifiCrate,
			// Token: 0x0400154C RID: 5452
			ScifiTable,
			// Token: 0x0400154D RID: 5453
			HauntedMirror,
			// Token: 0x0400154E RID: 5454
			Chains,
			// Token: 0x0400154F RID: 5455
			CandleStand,
			// Token: 0x04001550 RID: 5456
			Gravestone,
			// Token: 0x04001551 RID: 5457
			SlimeBackground,
			// Token: 0x04001552 RID: 5458
			Diploma,
			// Token: 0x04001553 RID: 5459
			WireFence,
			// Token: 0x04001554 RID: 5460
			WoodenBarrel,
			// Token: 0x04001555 RID: 5461
			MetalBarrel,
			// Token: 0x04001556 RID: 5462
			WindowFrameWooden,
			// Token: 0x04001557 RID: 5463
			YardLamp,
			// Token: 0x04001558 RID: 5464
			Bookshelf,
			// Token: 0x04001559 RID: 5465
			GreyBrickWallpaper,
			// Token: 0x0400155A RID: 5466
			RustyPlate,
			// Token: 0x0400155B RID: 5467
			JunkBackground,
			// Token: 0x0400155C RID: 5468
			FireBarrel,
			// Token: 0x0400155D RID: 5469
			JunkBlock,
			// Token: 0x0400155E RID: 5470
			WastelandWall,
			// Token: 0x0400155F RID: 5471
			BulletRiddledWall,
			// Token: 0x04001560 RID: 5472
			RustyBackground,
			// Token: 0x04001561 RID: 5473
			DeadTree,
			// Token: 0x04001562 RID: 5474
			RadioactiveBarrel,
			// Token: 0x04001563 RID: 5475
			UraniumBlock,
			// Token: 0x04001564 RID: 5476
			WindowFrameBroken,
			// Token: 0x04001565 RID: 5477
			DottedPinkBlock,
			// Token: 0x04001566 RID: 5478
			SheetMetalBlack,
			// Token: 0x04001567 RID: 5479
			SheetMetalDirty,
			// Token: 0x04001568 RID: 5480
			FishBowl,
			// Token: 0x04001569 RID: 5481
			PurpleJello,
			// Token: 0x0400156A RID: 5482
			CircuitBoard,
			// Token: 0x0400156B RID: 5483
			StarryWallpaper,
			// Token: 0x0400156C RID: 5484
			ManekiNekoR,
			// Token: 0x0400156D RID: 5485
			Bed,
			// Token: 0x0400156E RID: 5486
			BookPodium,
			// Token: 0x0400156F RID: 5487
			Gargoyle,
			// Token: 0x04001570 RID: 5488
			Fridge,
			// Token: 0x04001571 RID: 5489
			Coffin,
			// Token: 0x04001572 RID: 5490
			WindowFrameTinted,
			// Token: 0x04001573 RID: 5491
			WallpaperTorn,
			// Token: 0x04001574 RID: 5492
			WoodenBackgroundLight,
			// Token: 0x04001575 RID: 5493
			NoteBoard,
			// Token: 0x04001576 RID: 5494
			GlassDoorTinted,
			// Token: 0x04001577 RID: 5495
			Dice,
			// Token: 0x04001578 RID: 5496
			Gramophone,
			// Token: 0x04001579 RID: 5497
			GlassCabinet,
			// Token: 0x0400157A RID: 5498
			BarStool,
			// Token: 0x0400157B RID: 5499
			HelloBot,
			// Token: 0x0400157C RID: 5500
			WatermelonBlock,
			// Token: 0x0400157D RID: 5501
			ArrowSign,
			// Token: 0x0400157E RID: 5502
			StopSign,
			// Token: 0x0400157F RID: 5503
			PlayNoteA,
			// Token: 0x04001580 RID: 5504
			PlayNoteASharp,
			// Token: 0x04001581 RID: 5505
			PlayNoteB,
			// Token: 0x04001582 RID: 5506
			PlayNoteC,
			// Token: 0x04001583 RID: 5507
			PlayNoteCSharp,
			// Token: 0x04001584 RID: 5508
			PlayNoteD,
			// Token: 0x04001585 RID: 5509
			PlayNoteDSharp,
			// Token: 0x04001586 RID: 5510
			PlayNoteE,
			// Token: 0x04001587 RID: 5511
			PlayNoteF,
			// Token: 0x04001588 RID: 5512
			PlayNoteFSharp,
			// Token: 0x04001589 RID: 5513
			PlayNoteG,
			// Token: 0x0400158A RID: 5514
			PlayNoteGSharp,
			// Token: 0x0400158B RID: 5515
			RatingBoard,
			// Token: 0x0400158C RID: 5516
			MagicCauldron,
			// Token: 0x0400158D RID: 5517
			VortexPortal,
			// Token: 0x0400158E RID: 5518
			GlassBlock,
			// Token: 0x0400158F RID: 5519
			Fishtank,
			// Token: 0x04001590 RID: 5520
			FlatScreenTV,
			// Token: 0x04001591 RID: 5521
			WaterBed,
			// Token: 0x04001592 RID: 5522
			IronFence,
			// Token: 0x04001593 RID: 5523
			CactusBlock,
			// Token: 0x04001594 RID: 5524
			PottedPlant,
			// Token: 0x04001595 RID: 5525
			SkullBlock,
			// Token: 0x04001596 RID: 5526
			DiscoBall,
			// Token: 0x04001597 RID: 5527
			AmateurRadio,
			// Token: 0x04001598 RID: 5528
			DeskTopPC,
			// Token: 0x04001599 RID: 5529
			MetalChairYellow,
			// Token: 0x0400159A RID: 5530
			MetalChairBlue,
			// Token: 0x0400159B RID: 5531
			MetalChairRed,
			// Token: 0x0400159C RID: 5532
			MetalChairPink,
			// Token: 0x0400159D RID: 5533
			MetalChairGreen,
			// Token: 0x0400159E RID: 5534
			TVChair,
			// Token: 0x0400159F RID: 5535
			Sargophagus,
			// Token: 0x040015A0 RID: 5536
			TrashCan,
			// Token: 0x040015A1 RID: 5537
			HeartDecoration,
			// Token: 0x040015A2 RID: 5538
			OpenSign,
			// Token: 0x040015A3 RID: 5539
			DecorativeSword,
			// Token: 0x040015A4 RID: 5540
			MarbleFireplace,
			// Token: 0x040015A5 RID: 5541
			Bat,
			// Token: 0x040015A6 RID: 5542
			Bathtub,
			// Token: 0x040015A7 RID: 5543
			ParrotCage,
			// Token: 0x040015A8 RID: 5544
			StainedGlassWindow,
			// Token: 0x040015A9 RID: 5545
			Mailbox,
			// Token: 0x040015AA RID: 5546
			FireHydrant,
			// Token: 0x040015AB RID: 5547
			SwingChair,
			// Token: 0x040015AC RID: 5548
			MirrorWardrobe,
			// Token: 0x040015AD RID: 5549
			SuitOfArmor,
			// Token: 0x040015AE RID: 5550
			SteelBlock,
			// Token: 0x040015AF RID: 5551
			Portal,
			// Token: 0x040015B0 RID: 5552
			CeilingLampWhite,
			// Token: 0x040015B1 RID: 5553
			Fungi,
			// Token: 0x040015B2 RID: 5554
			CatDecoration,
			// Token: 0x040015B3 RID: 5555
			WallpaperWhiteStriped,
			// Token: 0x040015B4 RID: 5556
			CoatRack,
			// Token: 0x040015B5 RID: 5557
			DiscoBlock,
			// Token: 0x040015B6 RID: 5558
			Whiteboard,
			// Token: 0x040015B7 RID: 5559
			MammothIceBlock,
			// Token: 0x040015B8 RID: 5560
			PosterHeavyMetal,
			// Token: 0x040015B9 RID: 5561
			MakeupTable,
			// Token: 0x040015BA RID: 5562
			MetalTableRound,
			// Token: 0x040015BB RID: 5563
			Wardrobe,
			// Token: 0x040015BC RID: 5564
			CoatRackSheriff,
			// Token: 0x040015BD RID: 5565
			EndLavaRock,
			// Token: 0x040015BE RID: 5566
			EndLava,
			// Token: 0x040015BF RID: 5567
			TightsRed,
			// Token: 0x040015C0 RID: 5568
			TailTiger,
			// Token: 0x040015C1 RID: 5569
			ShirtFlash,
			// Token: 0x040015C2 RID: 5570
			ShirtHeart,
			// Token: 0x040015C3 RID: 5571
			PantsCamo,
			// Token: 0x040015C4 RID: 5572
			HatWolf,
			// Token: 0x040015C5 RID: 5573
			DressPink,
			// Token: 0x040015C6 RID: 5574
			ShoesLightBlue,
			// Token: 0x040015C7 RID: 5575
			ShoesLoafers,
			// Token: 0x040015C8 RID: 5576
			CoatLong,
			// Token: 0x040015C9 RID: 5577
			PantsSaggy,
			// Token: 0x040015CA RID: 5578
			ShoesUggBoots,
			// Token: 0x040015CB RID: 5579
			DressWhite,
			// Token: 0x040015CC RID: 5580
			ShirtSweaterWhite,
			// Token: 0x040015CD RID: 5581
			JacketBlack,
			// Token: 0x040015CE RID: 5582
			ShoesGroxsYellow,
			// Token: 0x040015CF RID: 5583
			ShirtJerseyPurple,
			// Token: 0x040015D0 RID: 5584
			ShoesFootwraps,
			// Token: 0x040015D1 RID: 5585
			HatFedoraGreen,
			// Token: 0x040015D2 RID: 5586
			PantsLeatherBlack,
			// Token: 0x040015D3 RID: 5587
			PantsShortsJean,
			// Token: 0x040015D4 RID: 5588
			PantsJeansGreen,
			// Token: 0x040015D5 RID: 5589
			GlovesWhite,
			// Token: 0x040015D6 RID: 5590
			HatFedoraRed,
			// Token: 0x040015D7 RID: 5591
			CoatRain,
			// Token: 0x040015D8 RID: 5592
			SuitOnepiece,
			// Token: 0x040015D9 RID: 5593
			HatHardHat,
			// Token: 0x040015DA RID: 5594
			ShirtSoccer,
			// Token: 0x040015DB RID: 5595
			ShirtAdventurer,
			// Token: 0x040015DC RID: 5596
			PantsAdventurer,
			// Token: 0x040015DD RID: 5597
			ShoesAdventurer,
			// Token: 0x040015DE RID: 5598
			PantsSoccer,
			// Token: 0x040015DF RID: 5599
			NoseClown,
			// Token: 0x040015E0 RID: 5600
			PantsCargo,
			// Token: 0x040015E1 RID: 5601
			PantsJeansRed,
			// Token: 0x040015E2 RID: 5602
			ShoesOnepiece,
			// Token: 0x040015E3 RID: 5603
			ShoesSneakersRed,
			// Token: 0x040015E4 RID: 5604
			EarPointy,
			// Token: 0x040015E5 RID: 5605
			HeadphonesBlack,
			// Token: 0x040015E6 RID: 5606
			HairBlue,
			// Token: 0x040015E7 RID: 5607
			HairLongBlack,
			// Token: 0x040015E8 RID: 5608
			HatAdventurer,
			// Token: 0x040015E9 RID: 5609
			BeardMoustache,
			// Token: 0x040015EA RID: 5610
			MaskCarneval,
			// Token: 0x040015EB RID: 5611
			MaskDevil,
			// Token: 0x040015EC RID: 5612
			GlassesMirrorShades,
			// Token: 0x040015ED RID: 5613
			GlassesSkiGogglesWhite,
			// Token: 0x040015EE RID: 5614
			SuitToga,
			// Token: 0x040015EF RID: 5615
			HatFedoraBlack,
			// Token: 0x040015F0 RID: 5616
			HatHeadScarfLightBlueHead,
			// Token: 0x040015F1 RID: 5617
			MaskMime,
			// Token: 0x040015F2 RID: 5618
			HatGlassHelmet,
			// Token: 0x040015F3 RID: 5619
			GloveMittensPink,
			// Token: 0x040015F4 RID: 5620
			SuitScifi,
			// Token: 0x040015F5 RID: 5621
			ShirtJerseyGreen,
			// Token: 0x040015F6 RID: 5622
			ShirtTSkullBlack,
			// Token: 0x040015F7 RID: 5623
			HairPunkGreen,
			// Token: 0x040015F8 RID: 5624
			DressLongBlue,
			// Token: 0x040015F9 RID: 5625
			TightsWhite,
			// Token: 0x040015FA RID: 5626
			SuitClown,
			// Token: 0x040015FB RID: 5627
			ShirtDressWhite,
			// Token: 0x040015FC RID: 5628
			ShirtBlouseRed,
			// Token: 0x040015FD RID: 5629
			ShoesBallerinaWhite,
			// Token: 0x040015FE RID: 5630
			SkirtGreen,
			// Token: 0x040015FF RID: 5631
			SkirtRed,
			// Token: 0x04001600 RID: 5632
			LockSmall,
			// Token: 0x04001601 RID: 5633
			LockMedium,
			// Token: 0x04001602 RID: 5634
			LockLarge,
			// Token: 0x04001603 RID: 5635
			LockWorld,
			// Token: 0x04001604 RID: 5636
			LockGold,
			// Token: 0x04001605 RID: 5637
			LockDiamond,
			// Token: 0x04001606 RID: 5638
			LockClan,
			// Token: 0x04001607 RID: 5639
			HatCapWoolWhite,
			// Token: 0x04001608 RID: 5640
			WorldKey,
			// Token: 0x04001609 RID: 5641
			CheckPoint,
			// Token: 0x0400160A RID: 5642
			BonusBox1,
			// Token: 0x0400160B RID: 5643
			BonusBox2,
			// Token: 0x0400160C RID: 5644
			BonusBox3,
			// Token: 0x0400160D RID: 5645
			BonusBoxVIP1,
			// Token: 0x0400160E RID: 5646
			PennantBlack,
			// Token: 0x0400160F RID: 5647
			SnowBlock,
			// Token: 0x04001610 RID: 5648
			IceBackground,
			// Token: 0x04001611 RID: 5649
			ShoesBallerinaRed,
			// Token: 0x04001612 RID: 5650
			ShoesBallerinaBlack,
			// Token: 0x04001613 RID: 5651
			JacketSuede,
			// Token: 0x04001614 RID: 5652
			DressSkaterYellow,
			// Token: 0x04001615 RID: 5653
			PantsJeansGanstaBaggy,
			// Token: 0x04001616 RID: 5654
			ShoesBasketballGansta,
			// Token: 0x04001617 RID: 5655
			ShirtJerseyGanstaRed,
			// Token: 0x04001618 RID: 5656
			NeckChainGanstaGold,
			// Token: 0x04001619 RID: 5657
			SkirtFarmDenim,
			// Token: 0x0400161A RID: 5658
			SuitFarmOveralls,
			// Token: 0x0400161B RID: 5659
			ShirtFarmPlaidRed,
			// Token: 0x0400161C RID: 5660
			WeaponSickleFarm,
			// Token: 0x0400161D RID: 5661
			WeaponMicrophoneGansta,
			// Token: 0x0400161E RID: 5662
			GlovesRingGanstaBlin,
			// Token: 0x0400161F RID: 5663
			PantsLeatherMedievalBrown,
			// Token: 0x04001620 RID: 5664
			TunicMedievalExecutioners,
			// Token: 0x04001621 RID: 5665
			TunicMedievalLords,
			// Token: 0x04001622 RID: 5666
			ShirtMedievalPeasantRags,
			// Token: 0x04001623 RID: 5667
			ShirtMedievalRingMail,
			// Token: 0x04001624 RID: 5668
			CapeMedievalLords,
			// Token: 0x04001625 RID: 5669
			GlovesMittensWoolWhite,
			// Token: 0x04001626 RID: 5670
			ShirtWoolWhite,
			// Token: 0x04001627 RID: 5671
			GloveClown,
			// Token: 0x04001628 RID: 5672
			ShoesClown,
			// Token: 0x04001629 RID: 5673
			PantsMedievalLords,
			// Token: 0x0400162A RID: 5674
			BonusArrowSign,
			// Token: 0x0400162B RID: 5675
			Buzzsaw,
			// Token: 0x0400162C RID: 5676
			BonusBlackBackground,
			// Token: 0x0400162D RID: 5677
			BonusBlackBlock,
			// Token: 0x0400162E RID: 5678
			BonusBlackBlockHole,
			// Token: 0x0400162F RID: 5679
			BonusBlackPillar,
			// Token: 0x04001630 RID: 5680
			BonusBoxVIP2,
			// Token: 0x04001631 RID: 5681
			BonusBoxVIP3,
			// Token: 0x04001632 RID: 5682
			BonusConcreteBackground,
			// Token: 0x04001633 RID: 5683
			BonusConcreteGrey,
			// Token: 0x04001634 RID: 5684
			BonusCushionBackground1,
			// Token: 0x04001635 RID: 5685
			BonusCushionBackground2,
			// Token: 0x04001636 RID: 5686
			BonusDarksBackground,
			// Token: 0x04001637 RID: 5687
			BonusFenceLeft,
			// Token: 0x04001638 RID: 5688
			BonusFenceMiddle,
			// Token: 0x04001639 RID: 5689
			BonusFenceRight,
			// Token: 0x0400163A RID: 5690
			BonusGrandPrize,
			// Token: 0x0400163B RID: 5691
			BonusLightbarsBackground,
			// Token: 0x0400163C RID: 5692
			BonusLightCeiling,
			// Token: 0x0400163D RID: 5693
			BonusLightWall,
			// Token: 0x0400163E RID: 5694
			BonusNumber1,
			// Token: 0x0400163F RID: 5695
			BonusNumber2,
			// Token: 0x04001640 RID: 5696
			BonusNumber3,
			// Token: 0x04001641 RID: 5697
			BonusNumber4,
			// Token: 0x04001642 RID: 5698
			BonusNumber5,
			// Token: 0x04001643 RID: 5699
			BonusRailing,
			// Token: 0x04001644 RID: 5700
			BonusRedBackground1,
			// Token: 0x04001645 RID: 5701
			BonusRedBackground2,
			// Token: 0x04001646 RID: 5702
			BonusShinyBlock,
			// Token: 0x04001647 RID: 5703
			BonusSign,
			// Token: 0x04001648 RID: 5704
			BonusStarsBackground,
			// Token: 0x04001649 RID: 5705
			BonusVioletBlock,
			// Token: 0x0400164A RID: 5706
			BonusBlueBackground1,
			// Token: 0x0400164B RID: 5707
			BonusBlueBackground2,
			// Token: 0x0400164C RID: 5708
			BonusVioletBackground1,
			// Token: 0x0400164D RID: 5709
			BonusVioletBackground2,
			// Token: 0x0400164E RID: 5710
			BonusOrangeBackground1,
			// Token: 0x0400164F RID: 5711
			BonusOrangeBackground2,
			// Token: 0x04001650 RID: 5712
			BonusBlueDotBlock,
			// Token: 0x04001651 RID: 5713
			BonusVioletDotBlock,
			// Token: 0x04001652 RID: 5714
			BonusPlatform,
			// Token: 0x04001653 RID: 5715
			Egg,
			// Token: 0x04001654 RID: 5716
			Milk,
			// Token: 0x04001655 RID: 5717
			TileRed,
			// Token: 0x04001656 RID: 5718
			TileOrange,
			// Token: 0x04001657 RID: 5719
			TileYellow,
			// Token: 0x04001658 RID: 5720
			TilePink,
			// Token: 0x04001659 RID: 5721
			TileBlue,
			// Token: 0x0400165A RID: 5722
			TileGreen,
			// Token: 0x0400165B RID: 5723
			TileGlass,
			// Token: 0x0400165C RID: 5724
			ArmChairLeopard,
			// Token: 0x0400165D RID: 5725
			CountryBlockBrazil,
			// Token: 0x0400165E RID: 5726
			CountryBlockDenmark,
			// Token: 0x0400165F RID: 5727
			CountryBlockFinland,
			// Token: 0x04001660 RID: 5728
			CountryBlockFrance,
			// Token: 0x04001661 RID: 5729
			CountryBlockGermany,
			// Token: 0x04001662 RID: 5730
			CountryBlockItaly,
			// Token: 0x04001663 RID: 5731
			CountryBlockNorway,
			// Token: 0x04001664 RID: 5732
			CountryBlockRussia,
			// Token: 0x04001665 RID: 5733
			CountryBlockSpain,
			// Token: 0x04001666 RID: 5734
			CountryBlockSweden,
			// Token: 0x04001667 RID: 5735
			CountryBlockUK,
			// Token: 0x04001668 RID: 5736
			ClassicSculpture,
			// Token: 0x04001669 RID: 5737
			JumpsuitMale,
			// Token: 0x0400166A RID: 5738
			CastleWallBackground,
			// Token: 0x0400166B RID: 5739
			Underwear,
			// Token: 0x0400166C RID: 5740
			FarmFence,
			// Token: 0x0400166D RID: 5741
			OrbDesertBackground,
			// Token: 0x0400166E RID: 5742
			OrbForestBackground,
			// Token: 0x0400166F RID: 5743
			OrbIceBackground,
			// Token: 0x04001670 RID: 5744
			OrbNightBackground,
			// Token: 0x04001671 RID: 5745
			OrbSpaceBackground,
			// Token: 0x04001672 RID: 5746
			OrbStarBackground,
			// Token: 0x04001673 RID: 5747
			TutorialBot,
			// Token: 0x04001674 RID: 5748
			JumpsuitFemale,
			// Token: 0x04001675 RID: 5749
			HatJumpsuitMale,
			// Token: 0x04001676 RID: 5750
			HatJumpsuitFemale,
			// Token: 0x04001677 RID: 5751
			BonusBigSign001,
			// Token: 0x04001678 RID: 5752
			BonusBigSign002,
			// Token: 0x04001679 RID: 5753
			BonusBigSign003,
			// Token: 0x0400167A RID: 5754
			BonusBigSign004,
			// Token: 0x0400167B RID: 5755
			BonusBigSign005,
			// Token: 0x0400167C RID: 5756
			BonusBigSign006,
			// Token: 0x0400167D RID: 5757
			BonusBigSign007,
			// Token: 0x0400167E RID: 5758
			BonusGrandPrizeLowerLeft,
			// Token: 0x0400167F RID: 5759
			BonusGrandPrizeLowerRight,
			// Token: 0x04001680 RID: 5760
			BonusGrandPrizeUpperLeft,
			// Token: 0x04001681 RID: 5761
			BonusGrandPrizeUpperRight,
			// Token: 0x04001682 RID: 5762
			HatSanta,
			// Token: 0x04001683 RID: 5763
			HairSanta,
			// Token: 0x04001684 RID: 5764
			ShoesSanta,
			// Token: 0x04001685 RID: 5765
			PantsSanta,
			// Token: 0x04001686 RID: 5766
			ShirtSanta,
			// Token: 0x04001687 RID: 5767
			BeardSanta,
			// Token: 0x04001688 RID: 5768
			MaskAlien1,
			// Token: 0x04001689 RID: 5769
			MaskAlien2,
			// Token: 0x0400168A RID: 5770
			SuitAlien1,
			// Token: 0x0400168B RID: 5771
			SuitAlien2,
			// Token: 0x0400168C RID: 5772
			ShirtHospitalGown,
			// Token: 0x0400168D RID: 5773
			ShoesSneakersPink,
			// Token: 0x0400168E RID: 5774
			ShoesSneakersGreen,
			// Token: 0x0400168F RID: 5775
			ShirtTanktopGreen,
			// Token: 0x04001690 RID: 5776
			ShirtTanktopBlack,
			// Token: 0x04001691 RID: 5777
			TightsBlack,
			// Token: 0x04001692 RID: 5778
			HatHeadScarfBlack,
			// Token: 0x04001693 RID: 5779
			HatHeadScarfRed,
			// Token: 0x04001694 RID: 5780
			ShirtBlouseOrange,
			// Token: 0x04001695 RID: 5781
			ShoesSuitAlien1,
			// Token: 0x04001696 RID: 5782
			ShoesSuitAlien2,
			// Token: 0x04001697 RID: 5783
			SkirtMaxiYellow,
			// Token: 0x04001698 RID: 5784
			DressMaxiLightGreen,
			// Token: 0x04001699 RID: 5785
			DressDecoMaxiLightGreen,
			// Token: 0x0400169A RID: 5786
			DressDecoLongBlue,
			// Token: 0x0400169B RID: 5787
			HatSlouchyBeanieGrey,
			// Token: 0x0400169C RID: 5788
			HeadphonesRed,
			// Token: 0x0400169D RID: 5789
			HeadphonesBlue,
			// Token: 0x0400169E RID: 5790
			PinkJello,
			// Token: 0x0400169F RID: 5791
			LightBlueJello,
			// Token: 0x040016A0 RID: 5792
			GlowingContainer,
			// Token: 0x040016A1 RID: 5793
			GlassesNerdyPurple,
			// Token: 0x040016A2 RID: 5794
			EarringsGold,
			// Token: 0x040016A3 RID: 5795
			CapeGreen,
			// Token: 0x040016A4 RID: 5796
			DressDecoSkaterYellow,
			// Token: 0x040016A5 RID: 5797
			GloveSuitAlien1,
			// Token: 0x040016A6 RID: 5798
			GloveSuitAlien2,
			// Token: 0x040016A7 RID: 5799
			WeaponGlowStickRed,
			// Token: 0x040016A8 RID: 5800
			WeaponGlowStickBlue,
			// Token: 0x040016A9 RID: 5801
			WeaponGlowStickGreen,
			// Token: 0x040016AA RID: 5802
			HairBuzzCutWhite,
			// Token: 0x040016AB RID: 5803
			HairLongOrange,
			// Token: 0x040016AC RID: 5804
			HairBlondeSpiky,
			// Token: 0x040016AD RID: 5805
			HairPonytailBlonde,
			// Token: 0x040016AE RID: 5806
			GloveLeather,
			// Token: 0x040016AF RID: 5807
			DressDecoWhite,
			// Token: 0x040016B0 RID: 5808
			WingsPixie,
			// Token: 0x040016B1 RID: 5809
			MiniatureSpaceship,
			// Token: 0x040016B2 RID: 5810
			ScifiCratePile,
			// Token: 0x040016B3 RID: 5811
			ScifiComputer,
			// Token: 0x040016B4 RID: 5812
			BonusDoorVIP,
			// Token: 0x040016B5 RID: 5813
			HairAdminJaakko,
			// Token: 0x040016B6 RID: 5814
			JacketAdminJaakko,
			// Token: 0x040016B7 RID: 5815
			PantsJeansAdminJaakko,
			// Token: 0x040016B8 RID: 5816
			ShoesSneakersAdminJaakko,
			// Token: 0x040016B9 RID: 5817
			WeaponKatanaAdminJaakko,
			// Token: 0x040016BA RID: 5818
			BackKatanaNoHiltAdminJaakko,
			// Token: 0x040016BB RID: 5819
			GlassesAdminJaakko,
			// Token: 0x040016BC RID: 5820
			WeaponSantaCane,
			// Token: 0x040016BD RID: 5821
			HeadphonesAdminJaakko,
			// Token: 0x040016BE RID: 5822
			DaHoodSign,
			// Token: 0x040016BF RID: 5823
			PileOfMoney,
			// Token: 0x040016C0 RID: 5824
			DollarsBackground,
			// Token: 0x040016C1 RID: 5825
			MoneyBackground,
			// Token: 0x040016C2 RID: 5826
			RedVelvetBackground,
			// Token: 0x040016C3 RID: 5827
			BlingBlingBlock,
			// Token: 0x040016C4 RID: 5828
			BackKatanaHiltAdminJaakko,
			// Token: 0x040016C5 RID: 5829
			ShoesRubberBootsRed,
			// Token: 0x040016C6 RID: 5830
			WingsDarkPixie,
			// Token: 0x040016C7 RID: 5831
			ContactLensesRed,
			// Token: 0x040016C8 RID: 5832
			GlassesScifi,
			// Token: 0x040016C9 RID: 5833
			StrawberryBlock,
			// Token: 0x040016CA RID: 5834
			PineappleBlock,
			// Token: 0x040016CB RID: 5835
			KiwiBlock,
			// Token: 0x040016CC RID: 5836
			ShoesAdminCommander,
			// Token: 0x040016CD RID: 5837
			GlovesAdminCommander,
			// Token: 0x040016CE RID: 5838
			SuitAdminCommander,
			// Token: 0x040016CF RID: 5839
			HatHelmetVisorUpAdminCommander,
			// Token: 0x040016D0 RID: 5840
			HatHelmetVisorDownAdminCommander,
			// Token: 0x040016D1 RID: 5841
			GloveMittensRed,
			// Token: 0x040016D2 RID: 5842
			GloveMittensGreen,
			// Token: 0x040016D3 RID: 5843
			PantsElf,
			// Token: 0x040016D4 RID: 5844
			ShoesElf,
			// Token: 0x040016D5 RID: 5845
			HatElf,
			// Token: 0x040016D6 RID: 5846
			CoatElf,
			// Token: 0x040016D7 RID: 5847
			CandyCaneBlock,
			// Token: 0x040016D8 RID: 5848
			GingerbreadBlock,
			// Token: 0x040016D9 RID: 5849
			HollyWreath,
			// Token: 0x040016DA RID: 5850
			Snowman,
			// Token: 0x040016DB RID: 5851
			ChristmasRibbonGreen,
			// Token: 0x040016DC RID: 5852
			ChristmasRibbonRed,
			// Token: 0x040016DD RID: 5853
			ChristmasTree,
			// Token: 0x040016DE RID: 5854
			WinterBells,
			// Token: 0x040016DF RID: 5855
			ChristmasWallpaperRed,
			// Token: 0x040016E0 RID: 5856
			ChristmasWallpaperBlue,
			// Token: 0x040016E1 RID: 5857
			PantsKrampus,
			// Token: 0x040016E2 RID: 5858
			ShoesKrampus,
			// Token: 0x040016E3 RID: 5859
			HairKrampus,
			// Token: 0x040016E4 RID: 5860
			HatHornsKrampus,
			// Token: 0x040016E5 RID: 5861
			ReindeerLights,
			// Token: 0x040016E6 RID: 5862
			ChristmasLights,
			// Token: 0x040016E7 RID: 5863
			CoatKrampus,
			// Token: 0x040016E8 RID: 5864
			ScarfRed,
			// Token: 0x040016E9 RID: 5865
			ScarfGreen,
			// Token: 0x040016EA RID: 5866
			EarMuffsRed,
			// Token: 0x040016EB RID: 5867
			Icicles,
			// Token: 0x040016EC RID: 5868
			WeaponCandyCane,
			// Token: 0x040016ED RID: 5869
			CapeFrost,
			// Token: 0x040016EE RID: 5870
			HatchWooden,
			// Token: 0x040016EF RID: 5871
			HatchMetal,
			// Token: 0x040016F0 RID: 5872
			Skulls,
			// Token: 0x040016F1 RID: 5873
			Spider,
			// Token: 0x040016F2 RID: 5874
			AlienEye,
			// Token: 0x040016F3 RID: 5875
			StitchedSkinBlock,
			// Token: 0x040016F4 RID: 5876
			GhostBackground,
			// Token: 0x040016F5 RID: 5877
			GutsBackground,
			// Token: 0x040016F6 RID: 5878
			CloudPlatform,
			// Token: 0x040016F7 RID: 5879
			ShirtHoodieGrey,
			// Token: 0x040016F8 RID: 5880
			DressSkaterLightBlue,
			// Token: 0x040016F9 RID: 5881
			GlassesEyepatch,
			// Token: 0x040016FA RID: 5882
			CapeTowel,
			// Token: 0x040016FB RID: 5883
			GlassesScifiRed,
			// Token: 0x040016FC RID: 5884
			GlassesScifiGreenVIP,
			// Token: 0x040016FD RID: 5885
			GlassesMonocle,
			// Token: 0x040016FE RID: 5886
			WeaponShortSwordGolden,
			// Token: 0x040016FF RID: 5887
			WeaponFlameSword,
			// Token: 0x04001700 RID: 5888
			WeaponSwordAdminMidnightWalker,
			// Token: 0x04001701 RID: 5889
			SuitAdminMidnightWalker,
			// Token: 0x04001702 RID: 5890
			CandyBlockGreen,
			// Token: 0x04001703 RID: 5891
			CandyBlockRed,
			// Token: 0x04001704 RID: 5892
			CandyBlockPink,
			// Token: 0x04001705 RID: 5893
			CandyBlockBlue,
			// Token: 0x04001706 RID: 5894
			CandyBlockCyan,
			// Token: 0x04001707 RID: 5895
			CandyBlockYellow,
			// Token: 0x04001708 RID: 5896
			CandyWatermelonBlock,
			// Token: 0x04001709 RID: 5897
			CandySpiralBlock,
			// Token: 0x0400170A RID: 5898
			MilkChocolateBlock,
			// Token: 0x0400170B RID: 5899
			DarkChocolateBlock,
			// Token: 0x0400170C RID: 5900
			CandyLaceBackground,
			// Token: 0x0400170D RID: 5901
			ChocolateBackground,
			// Token: 0x0400170E RID: 5902
			Cake,
			// Token: 0x0400170F RID: 5903
			LiquorishBlock,
			// Token: 0x04001710 RID: 5904
			CandyBackground,
			// Token: 0x04001711 RID: 5905
			DarkChocolateDecoratedBlock,
			// Token: 0x04001712 RID: 5906
			MaskRobbers,
			// Token: 0x04001713 RID: 5907
			ShirtPonchoLightGreen,
			// Token: 0x04001714 RID: 5908
			SuitJumpPrison,
			// Token: 0x04001715 RID: 5909
			HatCandyKing,
			// Token: 0x04001716 RID: 5910
			HatWinterPurple,
			// Token: 0x04001717 RID: 5911
			HatWonky,
			// Token: 0x04001718 RID: 5912
			MaskTeddyPink,
			// Token: 0x04001719 RID: 5913
			MaskTeddyBlue,
			// Token: 0x0400171A RID: 5914
			PantsShortsLove,
			// Token: 0x0400171B RID: 5915
			DressBallerina,
			// Token: 0x0400171C RID: 5916
			ShoesBallerinaLacedPink,
			// Token: 0x0400171D RID: 5917
			ShoesLeisure,
			// Token: 0x0400171E RID: 5918
			PantsLeisure,
			// Token: 0x0400171F RID: 5919
			ShirtLeisure,
			// Token: 0x04001720 RID: 5920
			PantsCandyShorts,
			// Token: 0x04001721 RID: 5921
			ShoesChoco,
			// Token: 0x04001722 RID: 5922
			ShoesCandyShoes,
			// Token: 0x04001723 RID: 5923
			WeaponLollipop,
			// Token: 0x04001724 RID: 5924
			ShoesHeffnerSlippers,
			// Token: 0x04001725 RID: 5925
			BeardMoustachePink,
			// Token: 0x04001726 RID: 5926
			HairCottonCandy,
			// Token: 0x04001727 RID: 5927
			HeartBlock,
			// Token: 0x04001728 RID: 5928
			GummyBearOrange,
			// Token: 0x04001729 RID: 5929
			GummyBearGreen,
			// Token: 0x0400172A RID: 5930
			GummyBearRed,
			// Token: 0x0400172B RID: 5931
			OrbCandyBackground,
			// Token: 0x0400172C RID: 5932
			CandyPillar,
			// Token: 0x0400172D RID: 5933
			CoatHeffner,
			// Token: 0x0400172E RID: 5934
			HairHairbandBlack,
			// Token: 0x0400172F RID: 5935
			HeartWallpaper,
			// Token: 0x04001730 RID: 5936
			DressCocktailBubblegum,
			// Token: 0x04001731 RID: 5937
			ShoesBubbleGum,
			// Token: 0x04001732 RID: 5938
			NeckRedRubyAdminEndless,
			// Token: 0x04001733 RID: 5939
			HairAdminEndless,
			// Token: 0x04001734 RID: 5940
			CoatAdminEndless,
			// Token: 0x04001735 RID: 5941
			MaskAdminEndless,
			// Token: 0x04001736 RID: 5942
			SuitTeddyPink,
			// Token: 0x04001737 RID: 5943
			SuitTeddyBlue,
			// Token: 0x04001738 RID: 5944
			ShoesTeddyPink,
			// Token: 0x04001739 RID: 5945
			ShoesTeddyBlue,
			// Token: 0x0400173A RID: 5946
			WingsCherubPink,
			// Token: 0x0400173B RID: 5947
			SuitOverallsCandy,
			// Token: 0x0400173C RID: 5948
			WeaponCandySceptre,
			// Token: 0x0400173D RID: 5949
			GlassesSunHeart,
			// Token: 0x0400173E RID: 5950
			CoatCandy,
			// Token: 0x0400173F RID: 5951
			HairPonytailRed,
			// Token: 0x04001740 RID: 5952
			HairLongPurple,
			// Token: 0x04001741 RID: 5953
			WeaponAdminBanHammer,
			// Token: 0x04001742 RID: 5954
			PantsCamoBlue,
			// Token: 0x04001743 RID: 5955
			HatSlouchyBeanieBlue,
			// Token: 0x04001744 RID: 5956
			ShoesSneakersWhite,
			// Token: 0x04001745 RID: 5957
			ShirtJerseyYellow,
			// Token: 0x04001746 RID: 5958
			CoatRainBlue,
			// Token: 0x04001747 RID: 5959
			ShirtTSkullBlue,
			// Token: 0x04001748 RID: 5960
			ShirtTanktopBlue,
			// Token: 0x04001749 RID: 5961
			SkirtYellow,
			// Token: 0x0400174A RID: 5962
			HatStetsonBeige,
			// Token: 0x0400174B RID: 5963
			ShirtTGrey,
			// Token: 0x0400174C RID: 5964
			CapeAchievementBlue,
			// Token: 0x0400174D RID: 5965
			AchievementMedalBronze,
			// Token: 0x0400174E RID: 5966
			AchievementMedalSilver,
			// Token: 0x0400174F RID: 5967
			AchievementMedalGold,
			// Token: 0x04001750 RID: 5968
			AchievementGobletBronze,
			// Token: 0x04001751 RID: 5969
			AchievementGobletSilver,
			// Token: 0x04001752 RID: 5970
			AchievementGobletGold,
			// Token: 0x04001753 RID: 5971
			MaskPlagueDoc,
			// Token: 0x04001754 RID: 5972
			PotOfGems,
			// Token: 0x04001755 RID: 5973
			CloverLeafBackground,
			// Token: 0x04001756 RID: 5974
			IrishBalloons,
			// Token: 0x04001757 RID: 5975
			LuckyHorseshoe,
			// Token: 0x04001758 RID: 5976
			CloverLeafBlock,
			// Token: 0x04001759 RID: 5977
			GreenGiftwrapBackground,
			// Token: 0x0400175A RID: 5978
			PennantGreen,
			// Token: 0x0400175B RID: 5979
			MushroomGreen,
			// Token: 0x0400175C RID: 5980
			RainbowBackground,
			// Token: 0x0400175D RID: 5981
			LeprechaunGnome,
			// Token: 0x0400175E RID: 5982
			GoldenHorseshoe,
			// Token: 0x0400175F RID: 5983
			LuckyCloverLeaf,
			// Token: 0x04001760 RID: 5984
			PotOfGold,
			// Token: 0x04001761 RID: 5985
			WindowClover,
			// Token: 0x04001762 RID: 5986
			HairAdminDev,
			// Token: 0x04001763 RID: 5987
			PantsSpandexGreen,
			// Token: 0x04001764 RID: 5988
			GlovesWristbandStPaddy,
			// Token: 0x04001765 RID: 5989
			HatStetsonGreen,
			// Token: 0x04001766 RID: 5990
			HairBobstyleGreen,
			// Token: 0x04001767 RID: 5991
			DressIrishMaid,
			// Token: 0x04001768 RID: 5992
			HatTophatIrish,
			// Token: 0x04001769 RID: 5993
			CoatLeprechaun,
			// Token: 0x0400176A RID: 5994
			PantsLeprechaun,
			// Token: 0x0400176B RID: 5995
			ShoesLeprechaun,
			// Token: 0x0400176C RID: 5996
			CoatGnome,
			// Token: 0x0400176D RID: 5997
			PantsShortsGnome,
			// Token: 0x0400176E RID: 5998
			GlassesRoundGlassesGreen,
			// Token: 0x0400176F RID: 5999
			ScarfIrish,
			// Token: 0x04001770 RID: 6000
			HairStPaddy,
			// Token: 0x04001771 RID: 6001
			BeardStPaddy,
			// Token: 0x04001772 RID: 6002
			ShoesGnome,
			// Token: 0x04001773 RID: 6003
			HatStPaddy,
			// Token: 0x04001774 RID: 6004
			WeaponFluteStPaddy,
			// Token: 0x04001775 RID: 6005
			IrishPennantString,
			// Token: 0x04001776 RID: 6006
			InfluencerWickerHat,
			// Token: 0x04001777 RID: 6007
			QuestNPC,
			// Token: 0x04001778 RID: 6008
			CloverLeaf,
			// Token: 0x04001779 RID: 6009
			WeaponSwordGallowglass,
			// Token: 0x0400177A RID: 6010
			HatBowlerLeprechaun,
			// Token: 0x0400177B RID: 6011
			WeaponStickLeprechaun,
			// Token: 0x0400177C RID: 6012
			CapeLeprechaunCape,
			// Token: 0x0400177D RID: 6013
			WingsCloverWings,
			// Token: 0x0400177E RID: 6014
			MaskIrishCharm,
			// Token: 0x0400177F RID: 6015
			NeckLuckyCharm,
			// Token: 0x04001780 RID: 6016
			WeaponGuitarAdminDev,
			// Token: 0x04001781 RID: 6017
			BeardAdminDev,
			// Token: 0x04001782 RID: 6018
			LockPlatinum,
			// Token: 0x04001783 RID: 6019
			HatHelmetLion,
			// Token: 0x04001784 RID: 6020
			HatCapBlack,
			// Token: 0x04001785 RID: 6021
			HatTophatDecoBlack,
			// Token: 0x04001786 RID: 6022
			HatBunnyEarsPink,
			// Token: 0x04001787 RID: 6023
			EasterBlockBlue,
			// Token: 0x04001788 RID: 6024
			EasterBlockGreen,
			// Token: 0x04001789 RID: 6025
			EasterBlockPurple,
			// Token: 0x0400178A RID: 6026
			EasterBlockRed,
			// Token: 0x0400178B RID: 6027
			EasterBlockYellow,
			// Token: 0x0400178C RID: 6028
			EasterSpheresBackground,
			// Token: 0x0400178D RID: 6029
			EasterStripesBackground,
			// Token: 0x0400178E RID: 6030
			EasterTilesBackground,
			// Token: 0x0400178F RID: 6031
			EasterEggDecorationOrange,
			// Token: 0x04001790 RID: 6032
			EasterEggDecorationBlue,
			// Token: 0x04001791 RID: 6033
			EasterEggDecorationViolet,
			// Token: 0x04001792 RID: 6034
			EasterEggBasket,
			// Token: 0x04001793 RID: 6035
			EasterEggTrophy,
			// Token: 0x04001794 RID: 6036
			WaterColorBlock,
			// Token: 0x04001795 RID: 6037
			BunnyPlushToy,
			// Token: 0x04001796 RID: 6038
			ChickPlushToy,
			// Token: 0x04001797 RID: 6039
			Serpentine,
			// Token: 0x04001798 RID: 6040
			SerpentineAndEggs,
			// Token: 0x04001799 RID: 6041
			TailEasterBunny,
			// Token: 0x0400179A RID: 6042
			NoseEasterBunny,
			// Token: 0x0400179B RID: 6043
			WeaponAxeEaster,
			// Token: 0x0400179C RID: 6044
			ShoesEasterBunny,
			// Token: 0x0400179D RID: 6045
			ShardGreen,
			// Token: 0x0400179E RID: 6046
			ShardRed,
			// Token: 0x0400179F RID: 6047
			ShardBlue,
			// Token: 0x040017A0 RID: 6048
			ShardYellow,
			// Token: 0x040017A1 RID: 6049
			ShardOrange,
			// Token: 0x040017A2 RID: 6050
			ShardClear,
			// Token: 0x040017A3 RID: 6051
			ShardPink,
			// Token: 0x040017A4 RID: 6052
			ShardGrey,
			// Token: 0x040017A5 RID: 6053
			ShardAir,
			// Token: 0x040017A6 RID: 6054
			ShardFire,
			// Token: 0x040017A7 RID: 6055
			ShardWater,
			// Token: 0x040017A8 RID: 6056
			ShardEarth,
			// Token: 0x040017A9 RID: 6057
			ShardSpirit,
			// Token: 0x040017AA RID: 6058
			ShardElectro,
			// Token: 0x040017AB RID: 6059
			ShardSilicon,
			// Token: 0x040017AC RID: 6060
			ShardDoom,
			// Token: 0x040017AD RID: 6061
			ShardAmber,
			// Token: 0x040017AE RID: 6062
			ShardPixie,
			// Token: 0x040017AF RID: 6063
			ShardCircuit,
			// Token: 0x040017B0 RID: 6064
			ShardMagic,
			// Token: 0x040017B1 RID: 6065
			ShardFusion,
			// Token: 0x040017B2 RID: 6066
			ShardEaster,
			// Token: 0x040017B3 RID: 6067
			ShardNightmare,
			// Token: 0x040017B4 RID: 6068
			ShardHeart,
			// Token: 0x040017B5 RID: 6069
			Replicator,
			// Token: 0x040017B6 RID: 6070
			OrbHalloweenTowerBackground,
			// Token: 0x040017B7 RID: 6071
			BlueprintHatHelmetVisorPWR,
			// Token: 0x040017B8 RID: 6072
			BlueprintGlovesPWR,
			// Token: 0x040017B9 RID: 6073
			BlueprintShoesPWR,
			// Token: 0x040017BA RID: 6074
			BlueprintSuitPWR,
			// Token: 0x040017BB RID: 6075
			BlueprintWeaponSwordLaserGreen,
			// Token: 0x040017BC RID: 6076
			BlueprintWeaponSwordLaserRed,
			// Token: 0x040017BD RID: 6077
			BlueprintWeaponSwordLaserBlue,
			// Token: 0x040017BE RID: 6078
			BlueprintOrbSpaceBackground,
			// Token: 0x040017BF RID: 6079
			BlueprintCapeDark,
			// Token: 0x040017C0 RID: 6080
			BlueprintMaskBunnyDark,
			// Token: 0x040017C1 RID: 6081
			BlueprintSuitBunnyDark,
			// Token: 0x040017C2 RID: 6082
			BlueprintShoesBunnyDark,
			// Token: 0x040017C3 RID: 6083
			BlueprintMaskTiki,
			// Token: 0x040017C4 RID: 6084
			BlueprintJetPackPlasma,
			// Token: 0x040017C5 RID: 6085
			BlueprintNecklaceGlimmer,
			// Token: 0x040017C6 RID: 6086
			BlueprintWeaponSwordFlaming,
			// Token: 0x040017C7 RID: 6087
			HatHelmetVisorPWR,
			// Token: 0x040017C8 RID: 6088
			GlovesPWR,
			// Token: 0x040017C9 RID: 6089
			ShoesPWR,
			// Token: 0x040017CA RID: 6090
			SuitPWR,
			// Token: 0x040017CB RID: 6091
			WeaponSwordLaserGreen,
			// Token: 0x040017CC RID: 6092
			WeaponSwordLaserRed,
			// Token: 0x040017CD RID: 6093
			WeaponSwordLaserBlue,
			// Token: 0x040017CE RID: 6094
			CapeDark,
			// Token: 0x040017CF RID: 6095
			MaskBunnyDark,
			// Token: 0x040017D0 RID: 6096
			SuitBunnyDark,
			// Token: 0x040017D1 RID: 6097
			ShoesBunnyDark,
			// Token: 0x040017D2 RID: 6098
			MaskTiki,
			// Token: 0x040017D3 RID: 6099
			JetPackPlasma,
			// Token: 0x040017D4 RID: 6100
			NecklaceGlimmer,
			// Token: 0x040017D5 RID: 6101
			ShirtHoodieSupport,
			// Token: 0x040017D6 RID: 6102
			WingsDragonBlue,
			// Token: 0x040017D7 RID: 6103
			JetPackSoda,
			// Token: 0x040017D8 RID: 6104
			LockWorldDark,
			// Token: 0x040017D9 RID: 6105
			MaskChick,
			// Token: 0x040017DA RID: 6106
			SuitChick,
			// Token: 0x040017DB RID: 6107
			ShoesChick,
			// Token: 0x040017DC RID: 6108
			MaskBunnyGreen,
			// Token: 0x040017DD RID: 6109
			SuitBunnyGreen,
			// Token: 0x040017DE RID: 6110
			ShoesBunnyGreen,
			// Token: 0x040017DF RID: 6111
			HairChick,
			// Token: 0x040017E0 RID: 6112
			CapeEasterWitch,
			// Token: 0x040017E1 RID: 6113
			HatBunnyEars,
			// Token: 0x040017E2 RID: 6114
			MaskEggDetector,
			// Token: 0x040017E3 RID: 6115
			HatEasterWitchHeadScarf,
			// Token: 0x040017E4 RID: 6116
			WeaponEasterBranch,
			// Token: 0x040017E5 RID: 6117
			ShoesEasterWitch,
			// Token: 0x040017E6 RID: 6118
			DressEasterWitch,
			// Token: 0x040017E7 RID: 6119
			WeaponEasterWitchBroom,
			// Token: 0x040017E8 RID: 6120
			GlovesSkiGlovesGreen,
			// Token: 0x040017E9 RID: 6121
			ShoesSkiBoots,
			// Token: 0x040017EA RID: 6122
			SuitOverallsSkiSuitRetro,
			// Token: 0x040017EB RID: 6123
			HairSkimaskedBlonde,
			// Token: 0x040017EC RID: 6124
			HairSkimaskedBrown,
			// Token: 0x040017ED RID: 6125
			HairSkimaskedBlack,
			// Token: 0x040017EE RID: 6126
			HairFringeSpikyBrown,
			// Token: 0x040017EF RID: 6127
			ContactLensesBlue,
			// Token: 0x040017F0 RID: 6128
			ContactLensesGreen,
			// Token: 0x040017F1 RID: 6129
			ContactLensesGold,
			// Token: 0x040017F2 RID: 6130
			ContactLensesBrown,
			// Token: 0x040017F3 RID: 6131
			ContactLensesSilver,
			// Token: 0x040017F4 RID: 6132
			ContactLensesPurple,
			// Token: 0x040017F5 RID: 6133
			ContactLensesWhite,
			// Token: 0x040017F6 RID: 6134
			ContactLensesPink,
			// Token: 0x040017F7 RID: 6135
			ContactLensesTurquoise,
			// Token: 0x040017F8 RID: 6136
			HairUndercutLongBlonde,
			// Token: 0x040017F9 RID: 6137
			HairUndercutLongBrown,
			// Token: 0x040017FA RID: 6138
			HairUndercutLongBlack,
			// Token: 0x040017FB RID: 6139
			HairUndercutLongRed,
			// Token: 0x040017FC RID: 6140
			HairUndercutWavyBrown,
			// Token: 0x040017FD RID: 6141
			HairUndercutWavyReddish,
			// Token: 0x040017FE RID: 6142
			HairUndercutWavyBlack,
			// Token: 0x040017FF RID: 6143
			HairUndercutWavyBlonde,
			// Token: 0x04001800 RID: 6144
			HairRockaBillyBlack,
			// Token: 0x04001801 RID: 6145
			HairJPopRed,
			// Token: 0x04001802 RID: 6146
			HairJPopBlue,
			// Token: 0x04001803 RID: 6147
			HairJPopPurple,
			// Token: 0x04001804 RID: 6148
			HairJPopGreen,
			// Token: 0x04001805 RID: 6149
			HairAfroBrown,
			// Token: 0x04001806 RID: 6150
			HairAfroBlack,
			// Token: 0x04001807 RID: 6151
			HairAfroReddish,
			// Token: 0x04001808 RID: 6152
			HairCurlyCurtainsBlonde,
			// Token: 0x04001809 RID: 6153
			HairCurlyCurtainsBlack,
			// Token: 0x0400180A RID: 6154
			HairCurlyCurtainsBrown,
			// Token: 0x0400180B RID: 6155
			HairEmoBlack,
			// Token: 0x0400180C RID: 6156
			GlovesRingFrost,
			// Token: 0x0400180D RID: 6157
			GlovesRingGoblin,
			// Token: 0x0400180E RID: 6158
			HairPuffyBlue,
			// Token: 0x0400180F RID: 6159
			HairPuffyRed,
			// Token: 0x04001810 RID: 6160
			HairSideyBrown,
			// Token: 0x04001811 RID: 6161
			HairSiippaLongBrown,
			// Token: 0x04001812 RID: 6162
			HairSiippaLongBlack,
			// Token: 0x04001813 RID: 6163
			HairSiippaLongRed,
			// Token: 0x04001814 RID: 6164
			HairZefBlonde,
			// Token: 0x04001815 RID: 6165
			HairZefBrown,
			// Token: 0x04001816 RID: 6166
			HairSpikyPunkBlue,
			// Token: 0x04001817 RID: 6167
			HairSpikyPunkRed,
			// Token: 0x04001818 RID: 6168
			HairMohawkGreen,
			// Token: 0x04001819 RID: 6169
			HairMohawkRed,
			// Token: 0x0400181A RID: 6170
			HairLongArchyBlonde,
			// Token: 0x0400181B RID: 6171
			HairLongArchyRed,
			// Token: 0x0400181C RID: 6172
			HairFringeSpikyBlonde,
			// Token: 0x0400181D RID: 6173
			HairFringeSpikyBlack,
			// Token: 0x0400181E RID: 6174
			HairFringeSpikyPink,
			// Token: 0x0400181F RID: 6175
			Deflector,
			// Token: 0x04001820 RID: 6176
			PinballBumper,
			// Token: 0x04001821 RID: 6177
			SpringBoard,
			// Token: 0x04001822 RID: 6178
			TrapdoorMetalPlatform,
			// Token: 0x04001823 RID: 6179
			PoisonTrap,
			// Token: 0x04001824 RID: 6180
			Elevator,
			// Token: 0x04001825 RID: 6181
			SpikeBall,
			// Token: 0x04001826 RID: 6182
			ShootingLaser,
			// Token: 0x04001827 RID: 6183
			TeslaSphere,
			// Token: 0x04001828 RID: 6184
			MovingPlatform,
			// Token: 0x04001829 RID: 6185
			PressurePlate,
			// Token: 0x0400182A RID: 6186
			ForceField,
			// Token: 0x0400182B RID: 6187
			GlueBlock,
			// Token: 0x0400182C RID: 6188
			GiftBox,
			// Token: 0x0400182D RID: 6189
			ScoreBoard,
			// Token: 0x0400182E RID: 6190
			FinishLine,
			// Token: 0x0400182F RID: 6191
			StartPoint,
			// Token: 0x04001830 RID: 6192
			DeathCounter,
			// Token: 0x04001831 RID: 6193
			CapeAdminMidnightWalkerDouble,
			// Token: 0x04001832 RID: 6194
			CapeAdminMidnightWalkerParachute,
			// Token: 0x04001833 RID: 6195
			MaskHorseHead,
			// Token: 0x04001834 RID: 6196
			OrientalTeaSet,
			// Token: 0x04001835 RID: 6197
			ToriiGate,
			// Token: 0x04001836 RID: 6198
			Hokora,
			// Token: 0x04001837 RID: 6199
			YinYangBlock,
			// Token: 0x04001838 RID: 6200
			SamuraiBlock,
			// Token: 0x04001839 RID: 6201
			SamuraiBackground,
			// Token: 0x0400183A RID: 6202
			TaikoDrum,
			// Token: 0x0400183B RID: 6203
			KatanaDecoration,
			// Token: 0x0400183C RID: 6204
			CherryBonsai,
			// Token: 0x0400183D RID: 6205
			Bamboo,
			// Token: 0x0400183E RID: 6206
			BambooWall,
			// Token: 0x0400183F RID: 6207
			ManekiNekoL,
			// Token: 0x04001840 RID: 6208
			DailyQuestNPC,
			// Token: 0x04001841 RID: 6209
			HatHelmetSamuraiRed,
			// Token: 0x04001842 RID: 6210
			ShirtSamuraiArmorRed,
			// Token: 0x04001843 RID: 6211
			PantsSamuraiArmorRedBlack,
			// Token: 0x04001844 RID: 6212
			ShoesSamuraiArmorYellowBlack,
			// Token: 0x04001845 RID: 6213
			HatHelmetSamuraiBlack,
			// Token: 0x04001846 RID: 6214
			ShirtSamuraiArmorBlack,
			// Token: 0x04001847 RID: 6215
			PantsSamuraiArmorRedYellow,
			// Token: 0x04001848 RID: 6216
			ShoesSamuraiArmorWhiteBrown,
			// Token: 0x04001849 RID: 6217
			MaskSamuraiRed,
			// Token: 0x0400184A RID: 6218
			MaskSamuraiBlack,
			// Token: 0x0400184B RID: 6219
			GloveNinjaPurple,
			// Token: 0x0400184C RID: 6220
			GloveNinjaGreyBlue,
			// Token: 0x0400184D RID: 6221
			GloveNinjaDarkRed,
			// Token: 0x0400184E RID: 6222
			HatHoodNinjaPurple,
			// Token: 0x0400184F RID: 6223
			HatHoodNinjaBlue,
			// Token: 0x04001850 RID: 6224
			MaskNinjaRed,
			// Token: 0x04001851 RID: 6225
			ShirtNinjaBlue,
			// Token: 0x04001852 RID: 6226
			ShirtNinjaPurple,
			// Token: 0x04001853 RID: 6227
			ShirtNinjaDarkRed,
			// Token: 0x04001854 RID: 6228
			PantsNinjaBlue,
			// Token: 0x04001855 RID: 6229
			PantsNinjaDark,
			// Token: 0x04001856 RID: 6230
			PantsNinjaGrey,
			// Token: 0x04001857 RID: 6231
			ShoesNinjaGrey,
			// Token: 0x04001858 RID: 6232
			ShoesNinjaRed,
			// Token: 0x04001859 RID: 6233
			ShoesNinjaPurple,
			// Token: 0x0400185A RID: 6234
			DressGeishaBlue,
			// Token: 0x0400185B RID: 6235
			DressGeishaRed,
			// Token: 0x0400185C RID: 6236
			HairSamurai,
			// Token: 0x0400185D RID: 6237
			HairShogun,
			// Token: 0x0400185E RID: 6238
			HairGeisha,
			// Token: 0x0400185F RID: 6239
			WeaponSai,
			// Token: 0x04001860 RID: 6240
			WeaponSamuraiKatana,
			// Token: 0x04001861 RID: 6241
			WeaponNaginata,
			// Token: 0x04001862 RID: 6242
			ShoesGeishaRed,
			// Token: 0x04001863 RID: 6243
			ShoesGeishaBlack,
			// Token: 0x04001864 RID: 6244
			PantsBrokenHoleBlack,
			// Token: 0x04001865 RID: 6245
			ShirtHoodieSupportFemale,
			// Token: 0x04001866 RID: 6246
			GlassesRetro,
			// Token: 0x04001867 RID: 6247
			TailDevil,
			// Token: 0x04001868 RID: 6248
			HatSteampunk,
			// Token: 0x04001869 RID: 6249
			BlueprintHatHelmetSamuraiBlack,
			// Token: 0x0400186A RID: 6250
			BlueprintMaskSamuraiBlack,
			// Token: 0x0400186B RID: 6251
			BlueprintShogunArmorShirt,
			// Token: 0x0400186C RID: 6252
			BlueprintShogunArmorPants,
			// Token: 0x0400186D RID: 6253
			BlueprintShogunShoes,
			// Token: 0x0400186E RID: 6254
			BlueprintShogunKatana,
			// Token: 0x0400186F RID: 6255
			WeaponShogunKatana,
			// Token: 0x04001870 RID: 6256
			CapeShogunRed,
			// Token: 0x04001871 RID: 6257
			BackDecorativeBackKatana,
			// Token: 0x04001872 RID: 6258
			BlueprintShogunCape,
			// Token: 0x04001873 RID: 6259
			HairBuzzcutBlack,
			// Token: 0x04001874 RID: 6260
			ShirtHoodieMod,
			// Token: 0x04001875 RID: 6261
			SandCastleSmall,
			// Token: 0x04001876 RID: 6262
			SandCastleMedium,
			// Token: 0x04001877 RID: 6263
			SandCastleLarge,
			// Token: 0x04001878 RID: 6264
			SunUmbrellaBlue,
			// Token: 0x04001879 RID: 6265
			SunUmbrellaRed,
			// Token: 0x0400187A RID: 6266
			SunUmbrellaGold,
			// Token: 0x0400187B RID: 6267
			ShirtSportsTopBlue,
			// Token: 0x0400187C RID: 6268
			ShirtSportsTopRed,
			// Token: 0x0400187D RID: 6269
			ShirtSportsTopGold,
			// Token: 0x0400187E RID: 6270
			PantsSpeedosBlue,
			// Token: 0x0400187F RID: 6271
			PantsSpeedosRed,
			// Token: 0x04001880 RID: 6272
			PantsSpeedosGolden,
			// Token: 0x04001881 RID: 6273
			GlassesSunBlue,
			// Token: 0x04001882 RID: 6274
			GlassesSunRed,
			// Token: 0x04001883 RID: 6275
			GlassesSunGolden,
			// Token: 0x04001884 RID: 6276
			NeckFloaterDuck,
			// Token: 0x04001885 RID: 6277
			NeckFloaterWalrus,
			// Token: 0x04001886 RID: 6278
			NeckFloaterDog,
			// Token: 0x04001887 RID: 6279
			ShoesFlippersBlue,
			// Token: 0x04001888 RID: 6280
			ShoesFlippersRed,
			// Token: 0x04001889 RID: 6281
			ShoesFlippersGold,
			// Token: 0x0400188A RID: 6282
			MaskSnorkelBlue,
			// Token: 0x0400188B RID: 6283
			MaskSnorkelRed,
			// Token: 0x0400188C RID: 6284
			MaskSnorkelGold,
			// Token: 0x0400188D RID: 6285
			WeaponGunWaterSmall,
			// Token: 0x0400188E RID: 6286
			WeaponGunWaterMedium,
			// Token: 0x0400188F RID: 6287
			WeaponGunWaterLarge,
			// Token: 0x04001890 RID: 6288
			WeaponSummerHammer,
			// Token: 0x04001891 RID: 6289
			CollectableQuestSummer,
			// Token: 0x04001892 RID: 6290
			QuestStarterItemSummer,
			// Token: 0x04001893 RID: 6291
			BreakableItemQuestSummer,
			// Token: 0x04001894 RID: 6292
			Fertilizer,
			// Token: 0x04001895 RID: 6293
			ShirtLifeVestOrange,
			// Token: 0x04001896 RID: 6294
			WeaponSurfboardGreen,
			// Token: 0x04001897 RID: 6295
			WeaponSurfboardYellow,
			// Token: 0x04001898 RID: 6296
			WeaponSurfboardPurple,
			// Token: 0x04001899 RID: 6297
			HairTrump,
			// Token: 0x0400189A RID: 6298
			LifeBuoy,
			// Token: 0x0400189B RID: 6299
			LifeGuardChair,
			// Token: 0x0400189C RID: 6300
			EntrancePortalMover,
			// Token: 0x0400189D RID: 6301
			HairAdminEndlessDeath,
			// Token: 0x0400189E RID: 6302
			ShirtTopAdminEndlessDeath,
			// Token: 0x0400189F RID: 6303
			GlassesAdminEndlessDeath,
			// Token: 0x040018A0 RID: 6304
			WristBandsAdminEndlessDeath,
			// Token: 0x040018A1 RID: 6305
			ShoesAdminEndlessDeath,
			// Token: 0x040018A2 RID: 6306
			PantsAdminEndlessDeath,
			// Token: 0x040018A3 RID: 6307
			WingsSongo,
			// Token: 0x040018A4 RID: 6308
			FamiliarGremlin1A,
			// Token: 0x040018A5 RID: 6309
			FamiliarGremlin2A,
			// Token: 0x040018A6 RID: 6310
			FamiliarGremlin3A,
			// Token: 0x040018A7 RID: 6311
			FamiliarGremlin4A,
			// Token: 0x040018A8 RID: 6312
			FamiliarGremlin4B,
			// Token: 0x040018A9 RID: 6313
			FamiliarGremlin5A,
			// Token: 0x040018AA RID: 6314
			FamiliarGremlin5C,
			// Token: 0x040018AB RID: 6315
			FamiliarCrow1A,
			// Token: 0x040018AC RID: 6316
			FamiliarCrow2A,
			// Token: 0x040018AD RID: 6317
			FamiliarBunny1A,
			// Token: 0x040018AE RID: 6318
			FamiliarBunny2A,
			// Token: 0x040018AF RID: 6319
			FamiliarBunny3A,
			// Token: 0x040018B0 RID: 6320
			FamiliarBunny4A,
			// Token: 0x040018B1 RID: 6321
			FamiliarBunny4B,
			// Token: 0x040018B2 RID: 6322
			FamiliarBot1A,
			// Token: 0x040018B3 RID: 6323
			FamiliarBot2A,
			// Token: 0x040018B4 RID: 6324
			FamiliarBot3A,
			// Token: 0x040018B5 RID: 6325
			FamiliarBot3B,
			// Token: 0x040018B6 RID: 6326
			FAMFoodCookieRed,
			// Token: 0x040018B7 RID: 6327
			FAMFoodCookieBlue,
			// Token: 0x040018B8 RID: 6328
			FAMFoodCookiePurple,
			// Token: 0x040018B9 RID: 6329
			FAMFoodCookieGreen,
			// Token: 0x040018BA RID: 6330
			FAMFoodCookieYellow,
			// Token: 0x040018BB RID: 6331
			FAMFoodCandyRed,
			// Token: 0x040018BC RID: 6332
			FAMFoodCandyBlue,
			// Token: 0x040018BD RID: 6333
			FAMFoodCandyPurple,
			// Token: 0x040018BE RID: 6334
			FAMFoodCandyGreen,
			// Token: 0x040018BF RID: 6335
			FAMFoodCandyYellow,
			// Token: 0x040018C0 RID: 6336
			FAMFoodJelloRed,
			// Token: 0x040018C1 RID: 6337
			FAMFoodJelloBlue,
			// Token: 0x040018C2 RID: 6338
			FAMFoodJelloPurple,
			// Token: 0x040018C3 RID: 6339
			FAMFoodJelloGreen,
			// Token: 0x040018C4 RID: 6340
			FAMFoodJelloYellow,
			// Token: 0x040018C5 RID: 6341
			FAMFoodSandwichRed,
			// Token: 0x040018C6 RID: 6342
			FAMFoodSandwichBlue,
			// Token: 0x040018C7 RID: 6343
			FAMFoodSandwichPurple,
			// Token: 0x040018C8 RID: 6344
			FAMFoodSandwichGreen,
			// Token: 0x040018C9 RID: 6345
			FAMFoodSandwichYellow,
			// Token: 0x040018CA RID: 6346
			WindowCastle,
			// Token: 0x040018CB RID: 6347
			FAMFoodMachine,
			// Token: 0x040018CC RID: 6348
			FAMEvolverator,
			// Token: 0x040018CD RID: 6349
			FamiliarNinjaPickle1A,
			// Token: 0x040018CE RID: 6350
			FamiliarWhale1A,
			// Token: 0x040018CF RID: 6351
			KiddieRide,
			// Token: 0x040018D0 RID: 6352
			LegendarySoilBlock,
			// Token: 0x040018D1 RID: 6353
			LockWorldBattle,
			// Token: 0x040018D2 RID: 6354
			LockBattle,
			// Token: 0x040018D3 RID: 6355
			BattleBarrierBasic,
			// Token: 0x040018D4 RID: 6356
			BattleScoreBoard,
			// Token: 0x040018D5 RID: 6357
			LockPart,
			// Token: 0x040018D6 RID: 6358
			BoneDust,
			// Token: 0x040018D7 RID: 6359
			FossilPuzzle,
			// Token: 0x040018D8 RID: 6360
			FossilTRexPart1,
			// Token: 0x040018D9 RID: 6361
			FossilTRexPart2,
			// Token: 0x040018DA RID: 6362
			FossilTRexPart3,
			// Token: 0x040018DB RID: 6363
			FossilTRexPart4,
			// Token: 0x040018DC RID: 6364
			FossilTRexPart5,
			// Token: 0x040018DD RID: 6365
			FossilTRexPart6,
			// Token: 0x040018DE RID: 6366
			FossilTRexPart7,
			// Token: 0x040018DF RID: 6367
			FossilTRexPart8,
			// Token: 0x040018E0 RID: 6368
			FossilTRexPart9,
			// Token: 0x040018E1 RID: 6369
			FossilAlligatorPart1,
			// Token: 0x040018E2 RID: 6370
			FossilAlligatorPart2,
			// Token: 0x040018E3 RID: 6371
			FossilAlligatorPart3,
			// Token: 0x040018E4 RID: 6372
			FossilAlligatorPart4,
			// Token: 0x040018E5 RID: 6373
			FossilAngelPart1,
			// Token: 0x040018E6 RID: 6374
			FossilAngelPart2,
			// Token: 0x040018E7 RID: 6375
			FossilAngelPart3,
			// Token: 0x040018E8 RID: 6376
			FossilAngelPart4,
			// Token: 0x040018E9 RID: 6377
			CheeseBlock,
			// Token: 0x040018EA RID: 6378
			Concrete1x1Block,
			// Token: 0x040018EB RID: 6379
			Concrete1x2Block,
			// Token: 0x040018EC RID: 6380
			Concrete2x2Block,
			// Token: 0x040018ED RID: 6381
			GlowBlockBlue,
			// Token: 0x040018EE RID: 6382
			GlowBlockGreen,
			// Token: 0x040018EF RID: 6383
			GlowBlockOrange,
			// Token: 0x040018F0 RID: 6384
			GlowBlockRed,
			// Token: 0x040018F1 RID: 6385
			HazardBlock,
			// Token: 0x040018F2 RID: 6386
			MetalStudded,
			// Token: 0x040018F3 RID: 6387
			ArmoredBackground,
			// Token: 0x040018F4 RID: 6388
			DiagonalCheckerBlack,
			// Token: 0x040018F5 RID: 6389
			DiagonalCheckerBlue,
			// Token: 0x040018F6 RID: 6390
			DiagonalCheckerRed,
			// Token: 0x040018F7 RID: 6391
			HerringboneTilesDirty,
			// Token: 0x040018F8 RID: 6392
			HerringboneTilesGrey,
			// Token: 0x040018F9 RID: 6393
			IllusionGreyBackground,
			// Token: 0x040018FA RID: 6394
			IllusionRedBackground,
			// Token: 0x040018FB RID: 6395
			JailBackground,
			// Token: 0x040018FC RID: 6396
			LavaBackground,
			// Token: 0x040018FD RID: 6397
			MetalBackground1,
			// Token: 0x040018FE RID: 6398
			MetalBackground2,
			// Token: 0x040018FF RID: 6399
			MetalBackground3,
			// Token: 0x04001900 RID: 6400
			MoireSquareBackground,
			// Token: 0x04001901 RID: 6401
			SpiralMosaic,
			// Token: 0x04001902 RID: 6402
			TileBlack,
			// Token: 0x04001903 RID: 6403
			UnslipperyMetal,
			// Token: 0x04001904 RID: 6404
			FenceWooden,
			// Token: 0x04001905 RID: 6405
			HousePlant,
			// Token: 0x04001906 RID: 6406
			OldWallLamp,
			// Token: 0x04001907 RID: 6407
			Vine,
			// Token: 0x04001908 RID: 6408
			ToiletSeat,
			// Token: 0x04001909 RID: 6409
			WeaponCleaver,
			// Token: 0x0400190A RID: 6410
			HatBucketRed,
			// Token: 0x0400190B RID: 6411
			BeardGoateeBlack,
			// Token: 0x0400190C RID: 6412
			HatHelmetVisorPWRRed,
			// Token: 0x0400190D RID: 6413
			GlovesPWRRed,
			// Token: 0x0400190E RID: 6414
			ShoesPWRRed,
			// Token: 0x0400190F RID: 6415
			SuitPWRRed,
			// Token: 0x04001910 RID: 6416
			BlueprintHatHelmetVisorPWRRed,
			// Token: 0x04001911 RID: 6417
			BlueprintGlovesPWRRed,
			// Token: 0x04001912 RID: 6418
			BlueprintShoesPWRRed,
			// Token: 0x04001913 RID: 6419
			BlueprintSuitPWRRed,
			// Token: 0x04001914 RID: 6420
			MoireRoundBackground,
			// Token: 0x04001915 RID: 6421
			GreenScreen,
			// Token: 0x04001916 RID: 6422
			HairLongNutturaBlack,
			// Token: 0x04001917 RID: 6423
			HairLongNutturaBrown,
			// Token: 0x04001918 RID: 6424
			HairEmoBlue,
			// Token: 0x04001919 RID: 6425
			HairEmoRed,
			// Token: 0x0400191A RID: 6426
			HairLongStripedBlackPurple,
			// Token: 0x0400191B RID: 6427
			WeaponBone,
			// Token: 0x0400191C RID: 6428
			HatHelmetBone,
			// Token: 0x0400191D RID: 6429
			SuitArmorBone,
			// Token: 0x0400191E RID: 6430
			WeaponMace,
			// Token: 0x0400191F RID: 6431
			HatHelmetVikingChainMail,
			// Token: 0x04001920 RID: 6432
			HatHelmetVikingSkyrim,
			// Token: 0x04001921 RID: 6433
			HatHelmetVikingTHorns,
			// Token: 0x04001922 RID: 6434
			HatHelmetVikingSimpleMasked,
			// Token: 0x04001923 RID: 6435
			HatHelmetVikingSideIron,
			// Token: 0x04001924 RID: 6436
			HatHelmetVikingWarlord,
			// Token: 0x04001925 RID: 6437
			HatHelmetVikingThor,
			// Token: 0x04001926 RID: 6438
			HatHoodVikingLadyBlonde,
			// Token: 0x04001927 RID: 6439
			HatHoodVikingLadyBrown,
			// Token: 0x04001928 RID: 6440
			HairVikingSideyBrown,
			// Token: 0x04001929 RID: 6441
			HairVikingSideyBlack,
			// Token: 0x0400192A RID: 6442
			HairVikingSideyBlonde,
			// Token: 0x0400192B RID: 6443
			HairVikingMaidenBraidFrontBlonde,
			// Token: 0x0400192C RID: 6444
			HairVikingMaidenBraidSideBlonde,
			// Token: 0x0400192D RID: 6445
			HairVikingMaidenBraidSideBrown,
			// Token: 0x0400192E RID: 6446
			HairVikingOpenBrown,
			// Token: 0x0400192F RID: 6447
			HairVikingOdinLongWhite,
			// Token: 0x04001930 RID: 6448
			BeardOdinLongWhite,
			// Token: 0x04001931 RID: 6449
			BeardVikingLongBrown,
			// Token: 0x04001932 RID: 6450
			BeardVikingBrown,
			// Token: 0x04001933 RID: 6451
			BeardVikingBlonde,
			// Token: 0x04001934 RID: 6452
			BeardVikingBlack,
			// Token: 0x04001935 RID: 6453
			BeardVikingSideburnsBrown,
			// Token: 0x04001936 RID: 6454
			FacialVikingMoustacheBrown,
			// Token: 0x04001937 RID: 6455
			ShoesVikingWarlord,
			// Token: 0x04001938 RID: 6456
			CoatVikingWarlord,
			// Token: 0x04001939 RID: 6457
			CapeVikingWarlord,
			// Token: 0x0400193A RID: 6458
			DressVikingShieldmaidenGreen,
			// Token: 0x0400193B RID: 6459
			ShoesVikingShieldmaidenGreen,
			// Token: 0x0400193C RID: 6460
			ShoesVikingBerserker,
			// Token: 0x0400193D RID: 6461
			PantsVikingBerserker,
			// Token: 0x0400193E RID: 6462
			ShirtVikingBerserker,
			// Token: 0x0400193F RID: 6463
			CapeVikingBerserker,
			// Token: 0x04001940 RID: 6464
			ShoesVikingThor,
			// Token: 0x04001941 RID: 6465
			CoatVikingThor,
			// Token: 0x04001942 RID: 6466
			CoatVikingSeer,
			// Token: 0x04001943 RID: 6467
			CoatDracula,
			// Token: 0x04001944 RID: 6468
			ShirtVikingWarriorChainmail,
			// Token: 0x04001945 RID: 6469
			ShoesVikingWarriorBrown,
			// Token: 0x04001946 RID: 6470
			ShirtVikingWarriorLeather,
			// Token: 0x04001947 RID: 6471
			ShoesVikingWarriorStrapped,
			// Token: 0x04001948 RID: 6472
			MaskMummy,
			// Token: 0x04001949 RID: 6473
			CoatVikingLady,
			// Token: 0x0400194A RID: 6474
			HairJHorror,
			// Token: 0x0400194B RID: 6475
			ShoesVikingLadyPurple,
			// Token: 0x0400194C RID: 6476
			HatVikingSwordThroughHead,
			// Token: 0x0400194D RID: 6477
			WeaponVikingAxeDouble,
			// Token: 0x0400194E RID: 6478
			WeaponVikingAxeGreat,
			// Token: 0x0400194F RID: 6479
			WeaponVikingAxeCurved,
			// Token: 0x04001950 RID: 6480
			WeaponVikingAxeMedium,
			// Token: 0x04001951 RID: 6481
			WeaponVikingSword,
			// Token: 0x04001952 RID: 6482
			WeaponVikingSpear,
			// Token: 0x04001953 RID: 6483
			WeaponVikingShieldRed,
			// Token: 0x04001954 RID: 6484
			WeaponVikingShieldGreen,
			// Token: 0x04001955 RID: 6485
			WeaponVikingShieldBlue,
			// Token: 0x04001956 RID: 6486
			WeaponVikingShieldThor,
			// Token: 0x04001957 RID: 6487
			WeaponVikingHammerThor,
			// Token: 0x04001958 RID: 6488
			BackhandItemVikingShield,
			// Token: 0x04001959 RID: 6489
			WingsValkyria,
			// Token: 0x0400195A RID: 6490
			VampireFangs,
			// Token: 0x0400195B RID: 6491
			VikingBlock,
			// Token: 0x0400195C RID: 6492
			VikingArmorBlock,
			// Token: 0x0400195D RID: 6493
			VikingWoodBackground,
			// Token: 0x0400195E RID: 6494
			VikingStoneBackground,
			// Token: 0x0400195F RID: 6495
			VikingRuneBackground,
			// Token: 0x04001960 RID: 6496
			VikingWoodenWall1,
			// Token: 0x04001961 RID: 6497
			VikingWoodenWall2,
			// Token: 0x04001962 RID: 6498
			VikingWoodenWall3,
			// Token: 0x04001963 RID: 6499
			VikingWoodenWall4,
			// Token: 0x04001964 RID: 6500
			RunestoneBlue,
			// Token: 0x04001965 RID: 6501
			RunestoneRed,
			// Token: 0x04001966 RID: 6502
			RunestoneGreen,
			// Token: 0x04001967 RID: 6503
			RunestoneOrange,
			// Token: 0x04001968 RID: 6504
			Bonfire,
			// Token: 0x04001969 RID: 6505
			VikingWeaponRack,
			// Token: 0x0400196A RID: 6506
			RavenTree,
			// Token: 0x0400196B RID: 6507
			VikingShieldDecoration,
			// Token: 0x0400196C RID: 6508
			VikingFigurehead,
			// Token: 0x0400196D RID: 6509
			BackhandItemVikingShieldGold,
			// Token: 0x0400196E RID: 6510
			BackhandItemVikingShieldIron,
			// Token: 0x0400196F RID: 6511
			BlueprintWingsValkyria,
			// Token: 0x04001970 RID: 6512
			WeaponSeerStaff,
			// Token: 0x04001971 RID: 6513
			OrbCemeteryBackground,
			// Token: 0x04001972 RID: 6514
			HairDracula,
			// Token: 0x04001973 RID: 6515
			GlovesRingDemon,
			// Token: 0x04001974 RID: 6516
			HatHornsDemon,
			// Token: 0x04001975 RID: 6517
			HatBrainsOut,
			// Token: 0x04001976 RID: 6518
			MaskJason,
			// Token: 0x04001977 RID: 6519
			MaskPumpkin,
			// Token: 0x04001978 RID: 6520
			CapeDracula,
			// Token: 0x04001979 RID: 6521
			DressCountessBathory,
			// Token: 0x0400197A RID: 6522
			HatBrownFedora,
			// Token: 0x0400197B RID: 6523
			ShirtKruger,
			// Token: 0x0400197C RID: 6524
			GlovesKrugerClaw,
			// Token: 0x0400197D RID: 6525
			PantsKruger,
			// Token: 0x0400197E RID: 6526
			MaskKruger,
			// Token: 0x0400197F RID: 6527
			WeaponScythe,
			// Token: 0x04001980 RID: 6528
			WeaponPoisonBlade,
			// Token: 0x04001981 RID: 6529
			WeaponBreadKnife,
			// Token: 0x04001982 RID: 6530
			DressMistressOfTheDark,
			// Token: 0x04001983 RID: 6531
			HairMistressOfTheDark,
			// Token: 0x04001984 RID: 6532
			MaskPaintSkull,
			// Token: 0x04001985 RID: 6533
			MaskCorpsePaint,
			// Token: 0x04001986 RID: 6534
			FamiliarGhost1A,
			// Token: 0x04001987 RID: 6535
			FamiliarSkull1A,
			// Token: 0x04001988 RID: 6536
			FamiliarPumpkin1A,
			// Token: 0x04001989 RID: 6537
			FamiliarEye1A,
			// Token: 0x0400198A RID: 6538
			MaskHoodBlack,
			// Token: 0x0400198B RID: 6539
			MaskPinhead,
			// Token: 0x0400198C RID: 6540
			CoatPinhead,
			// Token: 0x0400198D RID: 6541
			HairCountessBathory,
			// Token: 0x0400198E RID: 6542
			HairChucky,
			// Token: 0x0400198F RID: 6543
			OverallsChucky,
			// Token: 0x04001990 RID: 6544
			MaskChuckyScars,
			// Token: 0x04001991 RID: 6545
			PotionWerewolf,
			// Token: 0x04001992 RID: 6546
			CostumeWerewolf,
			// Token: 0x04001993 RID: 6547
			MaskPhantom,
			// Token: 0x04001994 RID: 6548
			MaskNamelessGhoul,
			// Token: 0x04001995 RID: 6549
			MaskIt,
			// Token: 0x04001996 RID: 6550
			MaskFrankenstein,
			// Token: 0x04001997 RID: 6551
			MaskCthulhu,
			// Token: 0x04001998 RID: 6552
			MaskSawBillyPuppet,
			// Token: 0x04001999 RID: 6553
			WingsDarkCherub,
			// Token: 0x0400199A RID: 6554
			MaskSkull,
			// Token: 0x0400199B RID: 6555
			ContactLensesSnake,
			// Token: 0x0400199C RID: 6556
			BlackCandles,
			// Token: 0x0400199D RID: 6557
			PumpkinLantern,
			// Token: 0x0400199E RID: 6558
			Ghost,
			// Token: 0x0400199F RID: 6559
			DungeonWallBars,
			// Token: 0x040019A0 RID: 6560
			OuijaBoard,
			// Token: 0x040019A1 RID: 6561
			ElectricChair,
			// Token: 0x040019A2 RID: 6562
			Spikes,
			// Token: 0x040019A3 RID: 6563
			ChurchBell,
			// Token: 0x040019A4 RID: 6564
			SpiderWeb,
			// Token: 0x040019A5 RID: 6565
			Blood,
			// Token: 0x040019A6 RID: 6566
			Acid,
			// Token: 0x040019A7 RID: 6567
			StonePlatform,
			// Token: 0x040019A8 RID: 6568
			TombStone,
			// Token: 0x040019A9 RID: 6569
			MimicCoffin,
			// Token: 0x040019AA RID: 6570
			CheckpointBonfire,
			// Token: 0x040019AB RID: 6571
			CapeMidnight,
			// Token: 0x040019AC RID: 6572
			WingsCthulhu,
			// Token: 0x040019AD RID: 6573
			BlueprintCapeMidnight,
			// Token: 0x040019AE RID: 6574
			CollectableLostSoulHalloween,
			// Token: 0x040019AF RID: 6575
			ZombieTrap,
			// Token: 0x040019B0 RID: 6576
			Fog,
			// Token: 0x040019B1 RID: 6577
			BattleBarrierBones,
			// Token: 0x040019B2 RID: 6578
			DemonAltar,
			// Token: 0x040019B3 RID: 6579
			SpiritCage,
			// Token: 0x040019B4 RID: 6580
			RuleBot,
			// Token: 0x040019B5 RID: 6581
			SpiralPillar,
			// Token: 0x040019B6 RID: 6582
			FamiliarGhost2A,
			// Token: 0x040019B7 RID: 6583
			FamiliarGhost2B,
			// Token: 0x040019B8 RID: 6584
			FamiliarSkull2A,
			// Token: 0x040019B9 RID: 6585
			Headstone,
			// Token: 0x040019BA RID: 6586
			CelticCross,
			// Token: 0x040019BB RID: 6587
			GraveSlant,
			// Token: 0x040019BC RID: 6588
			TowerGrandPrizeLowerLeft,
			// Token: 0x040019BD RID: 6589
			TowerGrandPrizeLowerRight,
			// Token: 0x040019BE RID: 6590
			TowerGrandPrizeUpperLeft,
			// Token: 0x040019BF RID: 6591
			TowerGrandPrizeUpperRight,
			// Token: 0x040019C0 RID: 6592
			END_OF_THE_ENUM
		}

		// Token: 0x02000236 RID: 566
		public enum GemType
		{
			// Token: 0x040019C6 RID: 6598
			Gem1,
			// Token: 0x040019C7 RID: 6599
			Gem2,
			// Token: 0x040019C8 RID: 6600
			Gem3,
			// Token: 0x040019C9 RID: 6601
			Gem4,
			// Token: 0x040019CA RID: 6602
			Gem5
		}

		// Token: 0x02000237 RID: 567
		public enum WorldLayoutType
		{
			// Token: 0x040019CC RID: 6604
			Basic,
			// Token: 0x040019CD RID: 6605
			BasicWithBots,
			// Token: 0x040019CE RID: 6606
			PerformanceTestFull,
			// Token: 0x040019CF RID: 6607
			PerformanceTestFullWithoutBots,
			// Token: 0x040019D0 RID: 6608
			PerformanceTestFullWithoutCollectables,
			// Token: 0x040019D1 RID: 6609
			PerformanceTestFullWithoutBotsAndCollectables,
			// Token: 0x040019D2 RID: 6610
			EmptyWhenIniting,
			// Token: 0x040019D3 RID: 6611
			PerformanceTestMedium,
			// Token: 0x040019D4 RID: 6612
			BasicWithCollectables,
			// Token: 0x040019D5 RID: 6613
			PerformanceTestFullWithoutBotsAndCollectablesAndTrees,
			// Token: 0x040019D6 RID: 6614
			PerformanceTestFullWithoutTrees,
			// Token: 0x040019D7 RID: 6615
			PerformanceTestMediumWithoutBots,
			// Token: 0x040019D8 RID: 6616
			PerformanceTestMediumWithoutCollectables,
			// Token: 0x040019D9 RID: 6617
			PerformanceTestMediumWithoutBotsAndCollectables,
			// Token: 0x040019DA RID: 6618
			PerformanceTestMediumWithoutBotsAndCollectablesAndTrees,
			// Token: 0x040019DB RID: 6619
			PerformanceTestMediumWithoutTrees,
			// Token: 0x040019DC RID: 6620
			HalloweenTower
		}

		public BSONObject Serialize();
    }
}
