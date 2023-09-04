<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
Me.SuspendLayout()
'
'OpenFileDialog
'
Me.OpenFileDialog.DefaultExt = "pdf"
Me.OpenFileDialog.Filter = "PDF file | *.pdf"
Me.OpenFileDialog.RestoreDirectory = True
Me.OpenFileDialog.ShowReadOnly = True
'
'Form1
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
Me.ClientSize = New System.Drawing.Size(927, 897)
Me.MinimumSize = New System.Drawing.Size(50, 50)
Me.Name = "Form1"
Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
Me.Text = "Form1"
Me.ResumeLayout(False)

End Sub
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog

End Class
