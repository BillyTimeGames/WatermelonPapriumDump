// Code by Hpman 
//
// https://twitter.com/The_Hpman/status/1442768809610387457
//
// Base logic for decoding 256 u16 blocks (512bytes)
// Key appears to vary within file, can't tie to some logic right now. Might be arbitrary?
// Might not cover whole file correctly

// $ mcs -out:DecodeXORBlocks.exe DecodeXORBlocks.cs

using System;
using System.IO;

namespace DecodeXORBlocks {
	class Decode {
		static void Main(string[] args) {
			Console.WriteLine("Reading S29GL064N binary dump!");

			byte[] bCoded;
			bCoded = File.ReadAllBytes(@"S29GL064N90BFI03@BGA48_20210924_142237.BIN");
			ushort[] coded = new ushort[bCoded.Length / 2];

			Console.WriteLine("Performing byte swap!");
			for (int i = 0; i < coded.Length; i++)
			{
				//coded[i] = (ushort)((bCoded[i * 2] << 8) + bCoded[i * 2 + 1]); //as in file load
				coded[i] = (ushort)((bCoded[i * 2 + 1] << 8) + bCoded[i * 2]); //byteswap load
			}

			Console.WriteLine("Attempting to XOR decode!");

			int endAddr = 0x800000 / 2;
			for (int addr = 0; addr < endAddr; addr++)
			{
				ushort key = 0x8a28;
				if (addr >= 0x4000 && addr < 0x8000) key = 0x228a;
				if ((addr & 0x01) != 0) key |= 0x0100; //bit 8
				if ((addr & 0x02) != 0) key |= 0x4000; //bit 14
				if ((addr & 0x04) != 0) key |= 0x0400; //bit 11
				if ((addr & 0x08) != 0) key |= 0x0040; //bit 6
				if ((addr & 0x10) != 0) key |= 0x0010; //bit 4
				if ((addr & 0x20) != 0) key |= 0x0001; //bit 0
				if ((addr & 0x40) != 0) key |= 0x1000; //bit 12
				if ((addr & 0x80) != 0) key |= 0x0004; //bit 2

				bCoded[addr * 2 + 1] = (byte)(coded[addr] ^ key);
				bCoded[addr * 2] = (byte)((coded[addr] ^ key) >> 8);
			}

			File.WriteAllBytes(@"S29GL064N90BFI03@BGA48_20210924_142237_DECODED.BIN", bCoded);

			Console.WriteLine("All done, press enter!");
			Console.ReadKey();
		}
	}
}


