﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataDrivenDesign
{
    [DisassemblyDiagnoser(printAsm: true)]
    public class Test
    {
        [Params(10_000, 100_000, 1_000_000, 10_000_000)]
        public int size;

        private SomeClass[] classes;
        private SomeStruct[] structs;
        private List<SomeClass> idiomatic;

        private float[] reds;
        private float[] greens;
        private float[] blues;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            classes = new SomeClass[size];
            structs = new SomeStruct[size];
            idiomatic = new List<SomeClass>(size);

            for (int i = 0; i < size; i++)
            {
                var smallvalue = (byte)random.Next(100);
                var red = random.Next(2) == 1;
                var green = random.Next(2) == 1;
                var blue = random.Next(2) == 1;
                var floatvalue = (float)random.NextDouble();

                var c = new SomeClass
                {
                    Name = "name",
                    Description = "description",
                    SmallValue = smallvalue,
                    IsRed = red,
                    IsGreen = green,
                    IsBlue = blue,
                    FloatValue = floatvalue,
                    Reference = null,
                    YetAnotherFlag = false,
                    RarelyUsedValue = 42,
                };

                classes[i] = c;
                idiomatic.Add(c);

                structs[i] = new SomeStruct
                {
                    Name = "name",
                    Description = "description",
                    SmallValue = smallvalue,
                    IsRed = red,
                    IsGreen = green,
                    IsBlue = blue,
                    FloatValue = floatvalue,
                    Reference = null,
                    YetAnotherFlag = false,
                    RarelyUsedValue = 42,
                };
            }

            reds = classes.Where(x => x.IsRed).Select(x => x.FloatValue).ToArray();
            greens = classes.Where(x => x.IsGreen).Select(x => x.FloatValue).ToArray();
            blues = classes.Where(x => x.IsBlue).Select(x => x.FloatValue).ToArray();
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public float UseClasses()
        {
            float r = 0, g = 0, b = 0;

            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i].IsRed) r += classes[i].FloatValue;
                if (classes[i].IsGreen) g += classes[i].FloatValue;
                if (classes[i].IsBlue) b += classes[i].FloatValue;
            }

            return r + g + b;
        }

        [Benchmark]
        public float UseStructs()
        {
            float r = 0, g = 0, b = 0;

            for (int i = 0; i < structs.Length; i++)
            {
                if (structs[i].IsRed) r += structs[i].FloatValue;
                if (structs[i].IsGreen) g += structs[i].FloatValue;
                if (structs[i].IsBlue) b += structs[i].FloatValue;
            }

            return r + g + b;
        }

        [Benchmark]
        public float DedicatedArrays()
        {
            float r = 0, g = 0, b = 0;

            for (int i = 0; i < reds.Length; i++) r += reds[i];
            for (int i = 0; i < greens.Length; i++) g += greens[i];
            for (int i = 0; i < blues.Length; i++) b += blues[i];

            return r + g + b;
        }

        [Benchmark]
        public float IdiomaticLINQ()
        {
            // Obviously slow as it iterates the collection 3 times
            float r = idiomatic.Where(x => x.IsRed).Sum(x => x.FloatValue);
            float g = idiomatic.Where(x => x.IsGreen).Sum(x => x.FloatValue);
            float b = idiomatic.Where(x => x.IsBlue).Sum(x => x.FloatValue);

            return r + g + b;
        }

        [Benchmark]
        public float IdiomaticForeach()
        {
            float r = 0, g = 0, b = 0;

            foreach (var c in idiomatic)
            {
                if (c.IsRed) r += c.FloatValue;
                if (c.IsGreen) g += c.FloatValue;
                if (c.IsBlue) b += c.FloatValue;
            }

            return r + g + b;
        }
    }

    public class SomeClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRed { get; set; }
        public bool IsGreen { get; set; }
        public bool IsBlue { get; set; }
        public float FloatValue { get; set; }
        public byte SmallValue { get; set; }
        public SomeClass Reference { get; set; }
        public bool YetAnotherFlag { get; set; }
        public double RarelyUsedValue { get; set; }

        // A lot of useful things you can do with SomeClass
    }

    public struct SomeStruct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRed { get; set; }
        public bool IsGreen { get; set; }
        public bool IsBlue { get; set; }
        public float FloatValue { get; set; }
        public byte SmallValue { get; set; }
        public SomeClass Reference { get; set; }
        public bool YetAnotherFlag { get; set; }
        public double RarelyUsedValue { get; set; }
    }
}
