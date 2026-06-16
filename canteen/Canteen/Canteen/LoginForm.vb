Imports System.Data.SqlClient

Public Class LoginForm

    Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load
        Me.Text = "KANTINA POS - Login"
        Me.Size = New Size(400, 500)
        Me.BackColor = Color.FromArgb(31, 82, 118)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
    End Sub

    Private Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnLogin.Click

        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()

        If username = "" Or password = "" Then
            MsgBox("Ilagay ang username at password!", MsgBoxStyle.Exclamation)
            Return
        End If

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand(
                    "SELECT UserID, FullName, Role FROM Users " & _
                    "WHERE Username = @User AND Password = @Pass " & _
                    "AND IsActive = 1", conn)
                cmd.Parameters.AddWithValue("@User", username)
                cmd.Parameters.AddWithValue("@Pass", password)
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                If dr.Read() Then
                    Dim userID As Integer = CInt(dr("UserID"))
                    Dim fullName As String = dr("FullName").ToString()
                    Dim role As String = dr("Role").ToString()
                    dr.Close()

                    Select Case role
                        Case "Cashier"
                            Dim frm As New KantinaPOS
                            frm.SetUser(userID, fullName)
                            frm.Show()
                            Me.Hide()

                        Case "Kitchen"
                            Dim frm As New KitchenDisplay
                            frm.Show()
                            Me.Hide()

                        Case "Admin"
                            Dim frm As New AdminPanel
                            frm.SetUser(userID, fullName) ' FIX: i-pass na ang admin user
                            frm.Show()
                            Me.Hide()
                    End Select
                Else
                    MsgBox("Mali ang username o password!", MsgBoxStyle.Critical)
                    txtPassword.Clear()
                    txtUsername.Focus()
                End If
            End Using

        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub btnExit_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub txtPassword_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) _
        Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnLogin_Click(sender, e)
        End If
    End Sub

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click

    End Sub

End Class