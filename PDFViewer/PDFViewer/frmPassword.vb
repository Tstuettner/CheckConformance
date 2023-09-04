Public Class frmPassword

ReadOnly Property Password() As String
   Get
      Return txtPassword.Text
   End Get
End Property

Private Sub txtPassword_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtPassword.KeyUp
   If e.KeyCode = Keys.Enter Then
      Me.DialogResult = DialogResult.OK
      Me.Close()
   End If
End Sub
End Class