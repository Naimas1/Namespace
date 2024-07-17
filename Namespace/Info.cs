using System;
using System.Collections.Generic;

public class Info
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public InfoStatus Status { get; set; } // New property for status
    public List<string> ImagesBase64 { get; set; } // List of base64 encoded images

    public Info()
    {
        ImagesBase64 = new List<string>();
    }
}
