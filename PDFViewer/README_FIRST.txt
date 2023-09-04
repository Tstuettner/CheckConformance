This example depends on the PDFCanvas control that must be installed
before the workspace PDFViewer can be compiled:

* On a 64 bit OS it is sometimes required to add the references of the
  workspaces ClassLibrary and PDFCanvas again to the workspace PDFViewer.
  Remove the existing references beforhand. The references are required
  if a symbol like DynaPDF is undefined. The workspace PDFViewer can now
  be compiled in most cases.

The following steps are only sometimes required, mostly on a 32 bit OS:

* Open the workspace PDFViewer.sln and activate the workspace PDFCanvas.

* Compile a Debug AND Release version now. VB requires both versions!

* Now you can activate the workspace PDFViewer and compile it. The new
  control PDFCanvas was automatically added to the Toolbox.

  * If you get compiler errors then check first whether you have really
    a debug AND release version of the PDFCanvas control compiled. If
    this was the case then add the PDFControl manually to the Toolbox:

	 * Open the Toolbox (Menu View/Toolbox), right click on an arbitrary
	   category and select "Choose Toolbox Items". Click on the Browse
	   button and select the file PDFViewer\bin\Release\CPDFCanvas.dll.
	   Click on Select and in the parent dialog on OK. Now you can
	   compile the workspace PDFViewer.

The PDFCanvas control was developed with C#, you find the source codes
in the directory examples/Visual CSharp/PDFViewer/PDFCanvas.

The C# interface of DynaPDF is encapsulated into a class library so that
it can be used with Visual Basic. You can work with the C# interface as
usual, also if you work with Visual Basic.

BTW - It is generally better to use the C# interface instead because
this version is much faster in comparison to the Visual Basic
counterpart...
