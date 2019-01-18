/*
 * Copyright (c) 2012 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO.Compression;

namespace Karaboss
{

    /** The possible PDF object types */
    public enum PDFType {
  PDFTypeCatalog, PDFTypePageTree, PDFTypePage, PDFTypeInfo, 
  PDFTypePageResource, PDFTypePageContent, PDFTypeLength, PDFTypeXObject
};

/** @class PDFObject
 * A PDF object represents an object in a PDF document.
 * It has a format like
 *
 *   5 0 obj
 *   << /Name /Value >>
 *   stream
 *   some data ....
 *   endstream
 *   endobj
 *
 * The first two numbers are the object's id number and generation
 * (generation is used for version tracking).
 *
 * Next is a 'PDF dictionary', a list of name/value string pairs.
 * Then, optionally, the object can have a stream of binary data.
 */
public class PDFObject {
    private int objectId;         /* The object id number (1, 2, 3 ..) */
    private PDFType pdftype;      /* The type of this object */
    private string stringpairs;   /* The name/value pairs */
    private byte[] streamvalue;   /* The binary stream value */
    private long offset;          /* The offset of this object in the PDF file */

    public int ObjectId {
        get { return objectId; }
    }

    public PDFType PDFType {
        get { return pdftype; }
    }

    public string StringPairs {
        get { return stringpairs; }
        set { stringpairs = value; }
    }

    public byte[] StreamValue {
        get { return streamvalue; }
        set { streamvalue = value; }
    }

    /* Return the string "<objectid> 0 R", which 
     * represents a reference to an object.
     */
    public string RefString {
        get { return "" + objectId + " 0 R"; }
    }

    /* The offset of this object in the PDF document.
     * The offset is initialized when we write this object to a file.
     */
    public long Offset {
        get { return offset; }
    }

    public PDFObject(int id, PDFType type) {
        objectId = id;
        pdftype = type;
    }

    /* Write this PDF object to the data stream. 
     * When the object is written, save the offset in the stream where it was written.
     * This is needed later on to generate the PDF xref (cross-reference) object table.
     */
    public void Write(Stream stream) {
        offset = stream.Position;     
        byte[] data = UTF8Encoding.UTF8.GetBytes(objectId.ToString() + " 0 obj\n");
        stream.Write(data, 0, data.Length);
        data = UTF8Encoding.UTF8.GetBytes(stringpairs);
        stream.Write(data, 0, data.Length);
        data = UTF8Encoding.UTF8.GetBytes("\n");
        stream.Write(data, 0, data.Length);
        if (streamvalue != null) {
            data = UTF8Encoding.UTF8.GetBytes("stream\n");
            stream.Write(data, 0, data.Length);
            stream.Write(streamvalue, 0, streamvalue.Length);
            data = UTF8Encoding.UTF8.GetBytes("endstream\n");
            stream.Write(data, 0, data.Length);
        }
        data = UTF8Encoding.UTF8.GetBytes("endobj\n");
        stream.Write(data, 0, data.Length);
    }
}


/** @class PDFWithImages
 *
 * The PDFWithImages class takes a set of images, and creates a single
 * PDF document out of them (one image per page).
 *
 * A PDF document has the following format:
 *
 * 1. The header (version and some binary data)
 *   %PDF-1.3
 *   %binary chars
 *
 * 2. The PDF objects
 *   2 0 obj
 *   << /Name /Value pairs >>
 *   endobj
 *
 * 3. A cross-reference (XRef) table.    
 *   This table gives the file offset of every PDF object in the file.
 *   The format is:
 *
 *   xref 0 17    // highest object number  
 *   0000000000 65535 f    // object 0, not used
 *   0000000023 00000 n    // object 1 offset
 *   0000000155 00000 n    // object 2 offset
 *   ....
 *
 * 4. A trailer 
 *
 *   It tells
 *   - The number of PDF objects (size)
 *   - The objectId of the Root 'catalog', the starting object of the document.
 *   - The objectId of the Info, which gives the document title/producer/create date.
 *   - The file offset of the XRef table
 *
 *   trailer
 *   << /Size /17 /Root 2 0 R /Info 1 0 R /ID [<timestamp> <timestamp> ] >>
 *   startxref 259454
 *   %%EOF
 *
 * The main PDF objects are:
 * 
 * - The document information
 *   << /Title file.mid /Producer (MidiSheetMusic) /CreationDate (D:YYYYMMDDHHMMSS-00 >>
 * 
 * - The catalog
 *   << /Type /Catalog /Pages 3 0 R /Version /1.4 >>
 *
 * - The page tree, which gives the document size, and list of pages
 *   << /Type /Pages /MediaBox [0 0 840 1090] /Count 2 /Kids [ 4 0 R 5 0 R ] >>
 *
 * - A single page, which consists of resources and contents
 *   << /Type /Page /Parent 3 0 R /Resources 6 0 R /Contents 7 0 R /MediaBox [0 0 840 1090] >>
 *
 * - The page resource points to the image to draw
 *   << /ProcSet [ /PDF /ImageB /ImageC /ImageI ] /XObject << /Im1 9 0 R >> >>
 *
 * - The page content gives the commands to draw the page (to draw the image in our case)
 *   << /Length 145 /Filter /FlateDecode >>
 *   stream
 *   q Q q /Perceptual ri q 840 0 0 1090 0 0 cm /Im1 Do Q Q  // zlib compressed
 *   endstream 
 *
 * - The XObject is the actual raw image to draw. 
 *   << /Length 159450 /Type /XObject /Subtype /Image /Width 840 /Height 1090 
 *      /Interpolate false /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode >>
 *   stream
 *   raw image data
 *   endstream
 * 
 *
 */
public class PDFWithImages {
    private string title;                /* Title of this sheet music */
    private Stream stream;               /* The stream to save the PDF to */
    private int numpages;                /* The number of pages in this PDF document */
    private List<PDFObject> pdfobjects;  /* The PDF objects */
    private long startxref;              /* The start offset of the XRef (cross-reference) table */

    public PDFWithImages(Stream stream, string title, int numpages) {
        this.stream = stream;
        this.title = title;
        this.numpages = numpages;
        pdfobjects = new List<PDFObject>();
        CreateHeaderObjects();
    }


    /** Compress the given bytes using zlib (deflate). 
     *  We have to manually add:
     *  - The two byte header (which gives the window size)
     *  - The 4-byte footer, which is an Adler-32 checksum
     */
    byte[] Compress(byte[] data) {
        MemoryStream output = new MemoryStream();
        output.WriteByte(0x58);
        output.WriteByte(0x85);
        DeflateStream stream = new DeflateStream(output, CompressionMode.Compress, true);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
        uint adler = Adler.Adler32(data);
        output.WriteByte((byte)( (adler >> 24) & 0xFF));
        output.WriteByte((byte)( (adler >> 16) & 0xFF));
        output.WriteByte((byte)( (adler >> 8) & 0xFF));
        output.WriteByte((byte)( adler & 0xFF));
        output.Close();
 
        byte[] result = output.ToArray(); 
        return result;
    }

    /** Compress the bitmap image using zlib deflate.
     *  The raw data format is 3 bytes per pixel (RGB), with the 
     *  pixels incrementing horizontally (left to right), row by row.
     *  top to bottom.
     */
    byte[] CompressImage(Bitmap bitmap) {
        byte[] rawimage = new byte[bitmap.Width * bitmap.Height * 3];
        int index = 0;
        for (int y = 0; y < bitmap.Height; y++) {
            for (int x = 0; x < bitmap.Width; x++) {
                Color color = bitmap.GetPixel(x, y);
                rawimage[index] = color.R; 
                rawimage[index+1] = color.G; 
                rawimage[index+2] = color.B; 
                index += 3;
            }
        }
        return Compress(rawimage);
    }


    /** Allocate a new PDF Object */
    PDFObject createPDFObject(PDFType type) {
        PDFObject obj = new PDFObject(pdfobjects.Count + 1, type);
        pdfobjects.Add(obj);
        return obj;
    }

    /** Retrieve the PDFObject with the given type */
    PDFObject getPDFObject(PDFType type) {
        foreach (PDFObject obj in pdfobjects) {
            if (obj.PDFType == type) {
                return obj;
            }
        }
        return null;
    } 

    /** Create the PDF catalog, info, and page tree objects */
    void CreateHeaderObjects() {
        string datestring = DateTime.Now.ToString("yyyymmddhhmmss-00'00'");
        PDFObject docinfo = createPDFObject(PDFType.PDFTypeInfo);
        docinfo.StringPairs = "<< /Title (" + title + ") /Producer (MidiSheetMusic) /CreationDate (D:" + datestring + ") >>";

        PDFObject catalog = createPDFObject(PDFType.PDFTypeCatalog);
        PDFObject pagetree = createPDFObject(PDFType.PDFTypePageTree);
        catalog.StringPairs = "<< /Type /Catalog /Pages " + pagetree.RefString + " /Version /1.4 >>";
    }


    /** Write the given string to the file stream */
    void WriteString(string s) {
        byte[] data = UTF8Encoding.UTF8.GetBytes(s);
        stream.Write(data, 0, data.Length);
    }

    /** Create a PDF page to display the given bitmap image.
     * We need:
     * - A page object, that points to the resources and contents of the page.
     * - A resource object, that defines Im1 (image 1), the image to draw.
     * - A content object, that says to draw Im1
     * - An XObject, that gives the raw image data (zlib-compressed)
     */
    public void AddImage(Bitmap bitmap) {
        PDFObject page = createPDFObject(PDFType.PDFTypePage);
        PDFObject pagetree = getPDFObject(PDFType.PDFTypePageTree);
        PDFObject pageResource = createPDFObject(PDFType.PDFTypePageResource);
        PDFObject pageContent = createPDFObject(PDFType.PDFTypePageContent);
        PDFObject pageContentLength = createPDFObject(PDFType.PDFTypeLength);
        PDFObject pageXObject = createPDFObject(PDFType.PDFTypeXObject);
        PDFObject pageXObjectLength = createPDFObject(PDFType.PDFTypeLength);
        
        page.StringPairs = "<< /Type /Page /Parent " + pagetree.RefString + 
                           " /Resources " + pageResource.RefString + 
                           " /Contents " + pageContent.RefString + 
                           " /MediaBox [0 0 840 1090] >>";

        pageResource.StringPairs = "<< /ProcSet [ /PDF /ImageB /ImageC /ImageI ] /XObject << /Im1 " + pageXObject.RefString + " >> >>";

        byte[] uncompressedValue = UTF8Encoding.UTF8.GetBytes("q Q q /Perceptual ri q " + bitmap.Width.ToString() + " 0 0 " + bitmap.Height.ToString() + " 0 0 cm /Im1 Do Q Q");
        byte[] compressedValue = Compress(uncompressedValue);
        pageContentLength.StringPairs = compressedValue.Length.ToString();
        pageContent.StringPairs = "<< /Length " + pageContentLength.RefString + " /Filter /FlateDecode >>";
        pageContent.StreamValue = compressedValue;

        pageXObject.StringPairs = "<< /Length " + pageXObjectLength.RefString + " /Type /XObject /Subtype /Image /Width " + bitmap.Width.ToString() + " /Height " + bitmap.Height.ToString() + " /Interpolate false /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode >>";
        pageXObject.StreamValue = CompressImage(bitmap);
        pageXObjectLength.StringPairs = pageXObject.StreamValue.Length.ToString();

    }


    /** Create the page tree object, which contains references to all the page objects */
    void initPageTreeValue() {
        PDFObject pagetree = getPDFObject(PDFType.PDFTypePageTree);
        string pageTreeValue = "<< /Type /Pages /MediaBox [0 0 840 1090] /Count " + numpages.ToString() + " /Kids [ ";
        foreach (PDFObject obj in pdfobjects) {
            if (obj.PDFType == PDFType.PDFTypePage) {
                pageTreeValue += obj.RefString + " ";
            }
        }
        pageTreeValue += "] >>";
        pagetree.StringPairs = pageTreeValue;
    }


    /** Write the Cross-Reference (XRef) table. */
    void WriteXRef() {
        startxref = stream.Position;
        WriteString("xref 0 " + (pdfobjects.Count + 1).ToString() + "\n");
        WriteString("0000000000 65535 f\n");
        foreach (PDFObject obj in pdfobjects) {
            string offsetString = obj.Offset.ToString();
            string pad = new string('0', 10 - offsetString.Length);
            WriteString(pad + offsetString + " 00000 n\n");
        } 
    }

    /** Write the PDF trailer */
    void WriteTrailer() {
        TimeSpan epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        int timestamp  = (int) epoch.TotalSeconds;
        PDFObject catalog = getPDFObject(PDFType.PDFTypeCatalog);
        PDFObject info = getPDFObject(PDFType.PDFTypeInfo);
        WriteString("trailer\n<< /Size " + (pdfobjects.Count + 1).ToString() + " /Root " + catalog.RefString + " /Info " + info.RefString + " /ID [ <" + timestamp.ToString() + "> <" + timestamp.ToString() + "> ] >>\n");
        WriteString("startxref\n" + startxref.ToString() + "\n%%EOF\n");
    }

    /** All the images have been added. Save this PDF document to the file stream */
    public void Save() {
        initPageTreeValue();

        WriteString("%PDF-1.3\n");
        byte[] data = new byte[5];
        data[0] = data[1] = data[2] = data[3] = 1;
        data[4] = (byte)'\n';
        stream.Write(data, 0, data.Length);

        foreach (PDFObject obj in pdfobjects) {
            obj.Write(stream);
        }

        WriteXRef();
        WriteTrailer();
    }
}

}
