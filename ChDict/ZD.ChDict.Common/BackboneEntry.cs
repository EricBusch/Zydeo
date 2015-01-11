﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace ZD.ChDict.Common
{
    /// <summary>
    /// A triplet or an English/German text and its two machine translations.
    /// </summary>
    public class TransTriple
    {
        /// <summary>
        /// The original text (English or German).
        /// </summary>
        public readonly string Orig;
        /// <summary>
        /// MT through Google.
        /// </summary>
        public readonly string Goog;
        /// <summary>
        /// MT through Bing.
        /// </summary>
        public readonly string Bing;
        /// <summary>
        /// Ctor: init immutable instance.
        /// </summary>
        public TransTriple(string orig, string goog, string bing)
        {
            if (string.IsNullOrEmpty(orig)) throw new ArgumentException("orig");
            if (string.IsNullOrEmpty(goog)) throw new ArgumentException("goog");
            if (string.IsNullOrEmpty(bing)) throw new ArgumentException("bing");
            Orig = orig;
            Goog = goog;
            Bing = bing;
        }
        /// <summary>
        /// Ctor: init all fields to same value.
        /// </summary>
        public TransTriple(string all)
        {
            if (string.IsNullOrEmpty(all)) throw new ArgumentException("all");
            Orig = all;
            Goog = all;
            Bing = all;
        }
    }

    public enum BackbonePart
    {
        /// <summary>
        /// Hungarian Wikipedia title (string or null).
        /// </summary>
        WikiHu,
        /// <summary>
        /// English Wikipedia title (<see cref="TransTriple"/> or null).
        /// </summary>
        WikiEn,
        /// <summary>
        /// German Wikipedia title (<see cref="TransTriple"/> or null).
        /// </summary>
        WikiDe,
        /// <summary>
        /// Cedict senses (array of <see cref="TransTriple"/> objects, or null).
        /// </summary>
        Cedict,
        /// <summary>
        /// HanDeDict senses (array of <see cref="TransTriple"/> objects, or null).
        /// </summary>
        HanDeDict,
    }

    /// <summary>
    /// Lexical backbone for a single dictionary entry.
    /// </summary>
    public class BackboneEntry
    {
        /// <summary>
        /// Headword: simplified Hanzi.
        /// </summary>
        public readonly string Simp;
        /// <summary>
        /// Headword: traditional Hanzi
        /// </summary>
        public readonly string Trad;
        /// <summary>
        /// Headword: Pinyin.
        /// </summary>
        public readonly string Pinyin;
        /// <summary>
        /// Rank in lexical scope.
        /// </summary>
        public readonly int Rank;
        /// <summary>
        /// Google MT of headword.
        /// </summary>
        public readonly string TransGoog;
        /// <summary>
        /// Bing MT of headword.
        /// </summary>
        public readonly string TransBing;

        /// <summary>
        /// All kinds of optional info in backbone.
        /// </summary>
        private readonly Dictionary<BackbonePart, object> storage = new Dictionary<BackbonePart,object>();

        /// <summary>
        /// Get one piece of backbone info.
        /// </summary>
        public object GetPart(BackbonePart part)
        {
            if (storage.ContainsKey(part)) return storage[part];
            return null;
        }

        /// <summary>
        /// Ctor: init immutable instance.
        /// </summary>
        public BackboneEntry(string simp, string trad, string pinyin, int rank,
            string transGoog, string transBing, Dictionary<BackbonePart, object> storage)
        {
            Simp = simp;
            Trad = trad;
            Pinyin = pinyin;
            Rank = rank;
            TransGoog = transGoog;
            TransBing = transBing;
            foreach (var x in storage) this.storage[x.Key] = x.Value;
        }

        public string WriteToXmlStr()
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings stgs = new XmlWriterSettings();
                stgs.OmitXmlDeclaration = true;
                stgs.Indent = true;
                stgs.IndentChars = "  ";
                stgs.NewLineChars = "\r\n";
                stgs.NewLineHandling = NewLineHandling.Replace;
                using (XmlWriter xw = XmlWriter.Create(sw, stgs))
                {
                    WriteToXml(xw);
                }
                return sw.ToString();
            }
        }

        /// <summary>
        /// Writes the entry into the provided XML stream.
        /// </summary>
        public void WriteToXml(XmlWriter xw)
        {
            // Start entry
            xw.WriteStartElement("entry");

            // Mandatory parts
            xw.WriteAttributeString("simp", Simp);
            xw.WriteAttributeString("trad", Trad);
            xw.WriteAttributeString("pinyin", Pinyin);
            xw.WriteElementString("rank", Rank.ToString());
            xw.WriteElementString("trans-goog", TransGoog);
            xw.WriteElementString("trans-bing", TransBing);

            // Wiki parts
            string wikiHu = GetPart(BackbonePart.WikiHu) as string;
            if (wikiHu != null) xw.WriteElementString("wiki-hu", wikiHu);
            TransTriple wikiEn = GetPart(BackbonePart.WikiEn) as TransTriple;
            if (wikiEn != null)
            {
                xw.WriteElementString("wiki-en-orig", wikiEn.Orig);
                xw.WriteElementString("wiki-en-goog", wikiEn.Goog);
                xw.WriteElementString("wiki-en-bing", wikiEn.Bing);
            }
            TransTriple wikiDe = GetPart(BackbonePart.WikiDe) as TransTriple;
            if (wikiDe != null)
            {
                xw.WriteElementString("wiki-de-orig", wikiDe.Orig);
                xw.WriteElementString("wiki-de-goog", wikiDe.Goog);
                xw.WriteElementString("wiki-de-bing", wikiDe.Bing);
            }

            // Cedict
            TransTriple[] cedict = GetPart(BackbonePart.Cedict) as TransTriple[];
            if (cedict != null)
            {
                xw.WriteStartElement("cedict");
                foreach(TransTriple tt in cedict)
                {
                    xw.WriteElementString("sense-orig", tt.Orig);
                    xw.WriteElementString("sense-goog", tt.Goog);
                    xw.WriteElementString("sense-bing", tt.Bing);
                }
                xw.WriteEndElement();
            }

            // HanDeDict
            TransTriple[] hdd = GetPart(BackbonePart.HanDeDict) as TransTriple[];
            if (hdd != null)
            {
                xw.WriteStartElement("handedict");
                foreach (TransTriple tt in hdd)
                {
                    xw.WriteElementString("sense-orig", tt.Orig);
                    xw.WriteElementString("sense-goog", tt.Goog);
                    xw.WriteElementString("sense-bing", tt.Bing);
                }
                xw.WriteEndElement();
            }

            // End entry
            xw.WriteEndElement();
        }

        public static BackboneEntry ReadFromXmlStr(string str)
        {
            using (StringReader sr = new StringReader(str))
            using (XmlTextReader xr = new XmlTextReader(sr))
            {
                xr.Read();
                return ReadFromXml(xr);
            }
        }

        private static void readDict(XmlTextReader xr, BackbonePart part, Dictionary<BackbonePart, object> storage)
        {
            List<TransTriple> senses = new List<TransTriple>();
            xr.Read();
            string orig = null;
            string goog = null;
            string bing = null;
            while (true)
            {
                if (xr.NodeType == XmlNodeType.Whitespace) { xr.Read(); continue; }
                if (xr.NodeType == XmlNodeType.EndElement) break;
                if (xr.NodeType != XmlNodeType.Element) throw new Exception("XML error.");
                if (xr.Name == "sense-orig") orig = xr.ReadElementContentAsString();
                else if (xr.Name == "sense-goog") goog = xr.ReadElementContentAsString();
                else if (xr.Name == "sense-bing")
                {
                    bing = xr.ReadElementContentAsString();
                    senses.Add(new TransTriple(orig, goog, bing));
                    orig = goog = bing = null;
                }
                else throw new Exception("XML error.");
            }
            storage[part] = senses.ToArray();
        }

        public static BackboneEntry ReadFromXml(XmlTextReader xr)
        {
            if (xr.NodeType != XmlNodeType.Element || xr.Name != "entry") return null;
            string simp = xr.GetAttribute("simp");
            string trad = xr.GetAttribute("trad");
            string pinyin = xr.GetAttribute("pinyin");
            int rank = -1;
            string transGoog = string.Empty;
            string transBing = string.Empty;
            Dictionary<BackbonePart, object> storage = new Dictionary<BackbonePart, object>();
            string wikiOrig = null;
            string wikiGoog = null;
            string wikiBing = null;
            while (true)
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.Whitespace) continue;
                if (xr.NodeType == XmlNodeType.EndElement) break;
                if (xr.NodeType != XmlNodeType.Element) throw new Exception("XML error.");
                if (xr.Name == "rank") rank = xr.ReadElementContentAsInt();
                else if (xr.Name == "trans-goog") transGoog = xr.ReadElementContentAsString();
                else if (xr.Name == "trans-bing") transBing = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-hu") storage[BackbonePart.WikiHu] = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-en-orig") wikiOrig = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-en-goog") wikiGoog = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-en-bing")
                {
                    wikiBing = xr.ReadElementContentAsString();
                    TransTriple tt = new TransTriple(wikiOrig, wikiGoog, wikiBing);
                    storage[BackbonePart.WikiEn] = tt;
                    wikiOrig = wikiGoog = wikiBing = null;
                }
                else if (xr.Name == "wiki-de-orig") wikiOrig = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-de-goog") wikiGoog = xr.ReadElementContentAsString();
                else if (xr.Name == "wiki-de-bing")
                {
                    wikiBing = xr.ReadElementContentAsString();
                    TransTriple tt = new TransTriple(wikiOrig, wikiGoog, wikiBing);
                    storage[BackbonePart.WikiDe] = tt;
                    wikiOrig = wikiGoog = wikiBing = null;
                }
                else if (xr.Name == "cedict")
                    readDict(xr, BackbonePart.Cedict, storage);
                else if (xr.Name == "handedict")
                    readDict(xr, BackbonePart.HanDeDict, storage);
                else xr.Skip();
            }
            xr.Read();
            while (xr.NodeType == XmlNodeType.Whitespace) xr.Read();
            return new BackboneEntry(simp, trad, pinyin, rank, transGoog, transBing, storage);
        }
    }
}
