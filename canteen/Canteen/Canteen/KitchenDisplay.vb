Imports System.Data.SqlClient

Public Class KitchenDisplay

    Private Sub KitchenDisplay_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load
        Me.Text = "Kitchen Display"
        Me.Size = New Size(900, 600)
        Me.BackColor = Color.FromArgb(28, 40, 51)
        Me.StartPosition = FormStartPosition.CenterScreen
        LoadOrders()
    End Sub

    ' =====================
    ' LOAD PENDING ORDERS
    ' =====================
    Private Sub LoadOrders()
        flpOrders.Controls.Clear()

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand(
                    "SELECT DISTINCT o.OrderID, o.OrderDate, o.Status, o.Notes " & _
                    "FROM Orders o " & _
                    "WHERE o.Status IN ('Pending', 'Preparing') " & _
                    "ORDER BY o.OrderDate ASC", conn)

                Dim dr As SqlDataReader = cmd.ExecuteReader()
                Dim orders As New List(Of Integer)

                While dr.Read()
                    orders.Add(CInt(dr("OrderID")))
                End While
                dr.Close()

                If orders.Count = 0 Then
                    Dim lblEmpty As New Label
                    lblEmpty.Text = "Walang pending orders..."
                    lblEmpty.ForeColor = Color.Gray
                    lblEmpty.Font = New Font("Arial", 14)
                    lblEmpty.Size = New Size(400, 50)
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter
                    flpOrders.Controls.Add(lblEmpty)
                    Return
                End If

                For Each orderID As Integer In orders
                    AddOrderCard(conn, orderID)
                Next
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' CREATE ORDER CARD
    ' =====================
    Private Sub AddOrderCard(ByVal conn As SqlConnection, ByVal orderID As Integer)
        Dim cmdOrder As New SqlCommand(
            "SELECT o.OrderID, o.Status, o.OrderDate, o.Notes " & _
            "FROM Orders o WHERE o.OrderID = @ID", conn)
        cmdOrder.Parameters.AddWithValue("@ID", orderID)
        Dim drOrder As SqlDataReader = cmdOrder.ExecuteReader()

        Dim status As String = "Pending"
        Dim orderDate As String = ""
        Dim notes As String = ""

        If drOrder.Read() Then
            status = drOrder("Status").ToString()
            orderDate = CDate(drOrder("OrderDate")).ToString("hh:mm tt")
            If Not IsDBNull(drOrder("Notes")) Then
                notes = drOrder("Notes").ToString()
            End If
        End If
        drOrder.Close()

        Dim cardColor As Color
        If status = "Pending" Then
            cardColor = Color.FromArgb(192, 57, 43)
        Else
            cardColor = Color.FromArgb(243, 156, 18)
        End If

        Dim pnl As New Panel
        pnl.Size = New Size(270, 260)
        pnl.BackColor = cardColor
        pnl.Margin = New Padding(8)

        Dim lblHeader As New Label
        lblHeader.Text = "ORDER #" & orderID & "  —  " & status.ToUpper()
        lblHeader.Font = New Font("Arial", 9, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.Size = New Size(260, 25)
        lblHeader.Location = New Point(5, 5)

        Dim lblTime As New Label
        lblTime.Text = "Time: " & orderDate
        lblTime.Font = New Font("Arial", 8)
        lblTime.ForeColor = Color.FromArgb(200, 200, 200)
        lblTime.Size = New Size(260, 18)
        lblTime.Location = New Point(5, 28)

        Dim lstItems As New ListBox
        lstItems.Size = New Size(258, 120)
        lstItems.Location = New Point(5, 50)
        lstItems.BackColor = Color.FromArgb(50, 50, 50)
        lstItems.ForeColor = Color.White
        lstItems.BorderStyle = BorderStyle.None
        lstItems.Font = New Font("Arial", 9)

        Dim cmdItems As New SqlCommand(
            "SELECT mi.ItemName, od.Quantity " & _
            "FROM OrderDetails od " & _
            "JOIN MenuItems mi ON od.ItemID = mi.ItemID " & _
            "WHERE od.OrderID = @ID", conn)
        cmdItems.Parameters.AddWithValue("@ID", orderID)
        Dim drItems As SqlDataReader = cmdItems.ExecuteReader()
        While drItems.Read()
            lstItems.Items.Add(drItems("ItemName").ToString() & _
                               "  x" & drItems("Quantity").ToString())
        End While
        drItems.Close()

        Dim lblNotes As New Label
        lblNotes.Text = If(notes <> "", "Note: " & notes, "")
        lblNotes.Font = New Font("Arial", 7, FontStyle.Italic)
        lblNotes.ForeColor = Color.White
        lblNotes.Size = New Size(258, 18)
        lblNotes.Location = New Point(5, 175)

        Dim btnPreparing As New Button
        btnPreparing.Text = "PREPARING"
        btnPreparing.Size = New Size(120, 32)
        btnPreparing.Location = New Point(5, 198)
        btnPreparing.BackColor = Color.FromArgb(243, 156, 18)
        btnPreparing.ForeColor = Color.White
        btnPreparing.FlatStyle = FlatStyle.Flat
        btnPreparing.Font = New Font("Arial", 8, FontStyle.Bold)
        btnPreparing.Tag = orderID
        AddHandler btnPreparing.Click, AddressOf BtnPreparing_Click

        ' FIX: READY button now marks as 'Completed' to finish the order flow
        Dim btnReady As New Button
        btnReady.Text = "READY / DONE"
        btnReady.Size = New Size(120, 32)
        btnReady.Location = New Point(140, 198)
        btnReady.BackColor = Color.FromArgb(39, 174, 96)
        btnReady.ForeColor = Color.White
        btnReady.FlatStyle = FlatStyle.Flat
        btnReady.Font = New Font("Arial", 8, FontStyle.Bold)
        btnReady.Tag = orderID
        AddHandler btnReady.Click, AddressOf BtnReady_Click

        pnl.Controls.AddRange(New Control() {
            lblHeader, lblTime, lstItems, lblNotes, btnPreparing, btnReady})
        flpOrders.Controls.Add(pnl)
    End Sub

    ' =====================
    ' UPDATE STATUS — PREPARING
    ' =====================
    Private Sub BtnPreparing_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim orderID As Integer = CInt(btn.Tag)
        UpdateOrderStatus(orderID, "Preparing")
    End Sub

    ' =====================
    ' UPDATE STATUS — COMPLETED (Ready/Done)
    ' FIX: Was 'Ready', now 'Completed' para lumabas sa Sales Report ng Admin
    ' =====================
    Private Sub BtnReady_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim orderID As Integer = CInt(btn.Tag)
        UpdateOrderStatus(orderID, "Completed")
    End Sub

    Private Sub UpdateOrderStatus(ByVal orderID As Integer, ByVal newStatus As String)
        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand(
                    "UPDATE Orders SET Status = @Status WHERE OrderID = @ID", conn)
                cmd.Parameters.AddWithValue("@Status", newStatus)
                cmd.Parameters.AddWithValue("@ID", orderID)
                cmd.ExecuteNonQuery()
            End Using
            MsgBox("Order #" & orderID & " — " & newStatus & "!", _
                   MsgBoxStyle.Information)
            LoadOrders()
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' REFRESH BUTTON
    ' =====================
    Private Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnRefresh.Click
        LoadOrders()
    End Sub

    ' =====================
    ' LOGOUT
    ' =====================
    Private Sub btnLogout_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnLogout.Click
        Me.Close()
        LoginForm.Show()
    End Sub

End Class