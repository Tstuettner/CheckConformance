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
Me.PictureBox1 = New System.Windows.Forms.PictureBox
CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
Me.SuspendLayout()
'
'PictureBox1
'
Me.PictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace
Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
Me.PictureBox1.Name = "PictureBox1"
Me.PictureBox1.Size = New System.Drawing.Size(927, 897)
Me.PictureBox1.TabIndex = 0
Me.PictureBox1.TabStop = False
'
'Form1
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.BackColor = System.Drawing.SystemColors.AppWorkspace
Me.ClientSize = New System.Drawing.Size(927, 897)
Me.Controls.Add(Me.PictureBox1)
Me.MinimumSize = New System.Drawing.Size(50, 50)
Me.Name = "Form1"
Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
Me.Text = "Form1"
CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
Me.ResumeLayout(False)

End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox

End Class
