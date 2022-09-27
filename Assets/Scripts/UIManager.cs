using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button import_Button;
    [SerializeField] Button warp_Button;
    [SerializeField] Button output_Button;
    [SerializeField] InputField rowSize_Input;
    [SerializeField] InputField seamMaxSize_Input;
    [SerializeField] Toggle horizontal_Toggle;
    [SerializeField] Toggle flip_Toggle;
    [SerializeField] Slider offset_Slider;
    [SerializeField] UISkin FileBrowserSkin;
    RawImage display_Image;
    Texture2D[] image_Seq;
    Texture2D display_Texture;
    [SerializeField] int rowSize = 4;
    [SerializeField] float seamMaxSize = 0.3f;
    float offset = 0;
    String currentPath;
    String importMessage = "Select Image Sequence";
    bool isHorizontal = true;
    bool isFlippedSide = false;

    void Start()
    {
        import_Button.onClick.AddListener(ImportImageSequence);
        warp_Button.onClick.AddListener(WarpImage);
        offset_Slider.onValueChanged.AddListener((value) =>
        {
            offset = value;
            WarpImage();
        });
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Images", ".jpg", ".jpeg", ".png"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.Skin = FileBrowserSkin;
        display_Image = GetComponent<RawImage>();
        rowSize_Input.text = "" + rowSize;
        seamMaxSize_Input.text = "" + seamMaxSize;
        rowSize_Input.gameObject.SetActive(false);
        seamMaxSize_Input.gameObject.SetActive(false);
    }

    void ImportImageSequence()
    {
        StartCoroutine(OpenFileBrowser());
    }

    public void switchOrientation(bool isHorizontal)
    {
        this.isHorizontal = isHorizontal;
        WarpImage();
    }

    public void switchSide(bool isFlippedSide)
    {
        this.isFlippedSide = isFlippedSide;
        WarpImage();
    }

    public void WarpImage()
    {
        if(isHorizontal)
        {
            WarpImageHorizontal();
        }
        else
        {
            WarpImageVertical();
        }
    }

    void WarpImageVertical()
    {
        // Image Size / Num of Images
        int colSize = image_Seq[0].height / image_Seq.Length;

        int rowOverflow = image_Seq[0].width % rowSize;

        int currentRowSize = rowSize;

        int offsetSize = (int) (image_Seq[0].height * offset);

        //Iterate over all rows
        for (int currentRowPosition = 0; currentRowPosition < image_Seq[0].width; currentRowPosition += rowSize)
        {
            //Change the size of the last row if there is an overflow
            if(currentRowPosition + currentRowSize > image_Seq[0].width)
            {
                currentRowSize = rowOverflow;
            }

            //Column position in current row
            int currentColPosition = 0;

            //Position of the currently used image in the array
            int numOfImage;

            //Iterate over all images to fill current row
            for (int i = 0; i < image_Seq.Length; i++)
            {
                //If the order of the images has been reversed, iterate over the array from the other direction
                if (!isFlippedSide)
                {
                    numOfImage = i;
                }
                else
                {
                    numOfImage = (image_Seq.Length - 1) - i;
                }

                //Calculate size of image strip to be inserted (randomized by 10%)
                int newColSize = colSize + (int)UnityEngine.Random.Range(colSize * -1f * seamMaxSize, colSize * seamMaxSize);

                newColSize += offsetSize;

                if(newColSize < 0)
                {
                    newColSize = 0;
                }
                //Since we use random values, we have to prevent overflow
                if ((currentColPosition + newColSize) > image_Seq[0].height)
                {
                    newColSize = image_Seq[0].height - currentColPosition;
                }
                Debug.Log("Num: " + numOfImage + " ColPos: " + currentColPosition + " newColPos: " + newColSize);

                //Get the pixel data of the current image
                Color[] texColor = image_Seq[numOfImage].GetPixels( currentRowPosition, currentColPosition, currentRowSize, newColSize );

                //and transfer them into the displayed Texture2D - they are not applyed yet to save resources
                display_Texture.SetPixels( currentRowPosition, currentColPosition , currentRowSize, newColSize, texColor);

                //shift column position for the next image strip
                currentColPosition += newColSize;

                //if the end of the row is already reached before the last image is taken into account, stop the loop
                if ((currentColPosition + newColSize) == image_Seq[0].height)
                {
                    i = image_Seq.Length;
                }
            }
            //if the end of the row has not been reached yet, fill row with data from last image
            if(currentColPosition < image_Seq[0].height)
            {
                if(!isFlippedSide)
                {
                    Color[] texColor = image_Seq[image_Seq.Length - 1].GetPixels(currentRowPosition, currentColPosition, rowSize, image_Seq[0].height - currentColPosition);
                    display_Texture.SetPixels(currentRowPosition, currentColPosition, currentRowSize, image_Seq[0].height - currentColPosition, texColor);
                }
                else
                {
                    Color[] texColor = image_Seq[0].GetPixels(currentRowPosition, currentColPosition, rowSize, image_Seq[0].height - currentColPosition);
                    display_Texture.SetPixels(currentRowPosition, currentColPosition, currentRowSize, image_Seq[0].height - currentColPosition, texColor);
                }
            }
        }
        //Apply the Texture2D data and set the Texture as the displayed Images Texture
        display_Texture.Apply();
        display_Image.texture = display_Texture;
    }

    void WarpImageHorizontal()
    {
        // Image Size / Num of Images
        int colSize = image_Seq[0].width / image_Seq.Length;

        int rowOverflow = image_Seq[0].height % rowSize;

        int currentRowSize = rowSize;

        int offsetSize = (int)(image_Seq[0].width * offset);

        //Iterate over all rows
        for (int currentRowPosition = 0; currentRowPosition < image_Seq[0].height; currentRowPosition += rowSize)
        {
            //Change the size of the last row if there is an overflow
            if (currentRowPosition + currentRowSize > image_Seq[0].height)
            {
                currentRowSize = rowOverflow;
            }

            //Column position in current row
            int currentColPosition = 0;

            //Position of the currently used image in the array
            int numOfImage;

            //Iterate over all images to fill current row
            for (int i = 0; i < image_Seq.Length; i++)
            {
                //If the order of the images has been reversed, iterate over the array from the other direction
                if (!isFlippedSide)
                {
                    numOfImage = i;
                }
                else
                {
                    numOfImage = (image_Seq.Length - 1) - i;
                }

                //Calculate size of image strip to be inserted (randomized by 10%)
                int newColSize = colSize + (int)UnityEngine.Random.Range(colSize * -1f * seamMaxSize, colSize * seamMaxSize);

                newColSize += offsetSize;

                if (newColSize < 0)
                {
                    newColSize = 0;
                }

                //Since we use random values, we have to prevent overflow
                if ((currentColPosition + newColSize) > image_Seq[0].width)
                {
                    newColSize = image_Seq[0].width - currentColPosition;
                }
                Debug.Log("Num: " + numOfImage + " ColPos: " + currentColPosition + " newColPos: " + newColSize);

                //Get the pixel data of the current image
                Color[] texColor = image_Seq[numOfImage].GetPixels(currentColPosition, currentRowPosition, newColSize, currentRowSize);

                //and transfer them into the displayed Texture2D - they are not applyed yet to save resources
                display_Texture.SetPixels(currentColPosition, currentRowPosition, newColSize, currentRowSize, texColor);

                //shift column position for the next image strip
                currentColPosition += newColSize;

                //if the end of the row is already reached before the last image is taken into account, stop the loop
                if ((currentColPosition + newColSize) == image_Seq[0].width)
                {
                    numOfImage = image_Seq.Length;
                }
            }
            //if the end of the row has not been reached yet, fill row with data from last image
            if (currentColPosition < image_Seq[0].width)
            {
                if(!isFlippedSide)
                {
                    Color[] texColor = image_Seq[image_Seq.Length - 1].GetPixels(currentColPosition, currentRowPosition, image_Seq[0].width - currentColPosition, rowSize);
                    display_Texture.SetPixels(currentColPosition, currentRowPosition, image_Seq[0].width - currentColPosition, currentRowSize, texColor);
                }
                else
                {
                    Color[] texColor = image_Seq[0].GetPixels(currentColPosition, currentRowPosition, image_Seq[0].width - currentColPosition, rowSize);
                    display_Texture.SetPixels(currentColPosition, currentRowPosition, image_Seq[0].width - currentColPosition, currentRowSize, texColor);
                }
            }
        }
        //Apply the Texture2D data and set the Texture as the displayed Images Texture
        display_Texture.Apply();
        display_Image.texture = display_Texture;
    }

    IEnumerator OpenFileBrowser()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, importMessage, "Import");
        if (FileBrowser.Success)
        {
            image_Seq = new Texture2D[FileBrowser.Result.Length];

            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            
            currentPath = FileBrowser.Result[0].Substring(0, FileBrowser.Result[0].LastIndexOf('\\'));

            string noNumPath = FileBrowser.Result[0].Substring(0, FileBrowser.Result[0].LastIndexOf('_') + 1);

            // Sort the images by their number
            Array.Sort<String>(FileBrowser.Result, new Comparison<String>(
                  (i1, i2) => i1.Substring(FileBrowser.Result[0].LastIndexOf('_')).CompareTo(i2.Substring(FileBrowser.Result[0].LastIndexOf('_')))));
            try
            {
                for (int i = 0; i < FileBrowser.Result.Length; i++)
                {
                    image_Seq[i] = LoadPNG(FileBrowser.Result[i]);
                    if(image_Seq[i].width != image_Seq[0].width || image_Seq[i].height != image_Seq[0].height)
                    {
                        throw new Exception();
                    }
                }

                display_Texture = new Texture2D(image_Seq[0].width, image_Seq[0].height);
                display_Image.texture = image_Seq[0];
                warp_Button.gameObject.SetActive(true);
                output_Button.gameObject.SetActive(true);
                rowSize_Input.gameObject.SetActive(true);
                seamMaxSize_Input.gameObject.SetActive(true);
                horizontal_Toggle.gameObject.SetActive(true);
                flip_Toggle.gameObject.SetActive(true);
                offset_Slider.gameObject.SetActive(true);
                importMessage = "Select Image Sequence";
                if(image_Seq[0].width <= 1920 && image_Seq[0].height <= 1080)
                {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(image_Seq[0].width, image_Seq[0].height);
                }
                else if(image_Seq[0].width > 1920 && image_Seq[0].height <= 1080)
                {

                }
                else if (image_Seq[0].width <= 1920 && image_Seq[0].height > 1080)
                {

                }
                else if (image_Seq[0].width <= 1920 && image_Seq[0].height > 1080)
                {

                }
            }
            catch (Exception e)
            {
                importMessage = "Invalid Image Sequence - Images have to be of same size and name ending with increasing \"_number\"";
                ImportImageSequence();
                Debug.Log("Invalid Images");
            }
        }
        else
        {
            currentPath = "";
            horizontal_Toggle.gameObject.SetActive(false);
            flip_Toggle.gameObject.SetActive(false);
            warp_Button.gameObject.SetActive(false);
            output_Button.gameObject.SetActive(false);
            rowSize_Input.gameObject.SetActive(false);
            seamMaxSize_Input.gameObject.SetActive(false);
            offset_Slider.gameObject.SetActive(false);
        }
    }
    static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData, false); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public void changeRowSize()
    {
        try
        {
            int newRowSize = int.Parse(rowSize_Input.text);
            if(newRowSize < 0 || newRowSize > 20)
            {
                throw new Exception();
            }
            rowSize = newRowSize;
            WarpImage();
        }
        catch (Exception e)
        {
            rowSize_Input.text = "Invalid";
        }
    }

    public void changeSeamMaxSize()
    {
        try
        {
            float newSeamSize = float.Parse(seamMaxSize_Input.text);
            if (newSeamSize <= 0f || newSeamSize > 1f)
            {
                throw new Exception();
            }
            seamMaxSize = newSeamSize;
            WarpImage();
        }
        catch (Exception e)
        {
            seamMaxSize_Input.text = "Invalid";
        }
    }

    public void saveImage()
    {
        //Get the RawImage Texture2D and encode it into a byteArray
        Texture2D itemBGTex = display_Image.texture as Texture2D;
        byte[] itemBGBytes = itemBGTex.EncodeToPNG();

        //Create the "Warped Images" Directory right next to the images
        Directory.CreateDirectory(currentPath + "/Warped Images/");

        //Write the image as .png with increasing file name number
        int imgNum = 1;
        while (File.Exists(currentPath + "/Warped Images/Warped_" + imgNum + ".png"))
        {
            imgNum++;
        }
        File.WriteAllBytes(currentPath + "/Warped Images/Warped_" + imgNum + ".png", itemBGBytes);
    }
}
