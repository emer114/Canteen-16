Imports System.Data.SqlClient

Public Class AdminPanel

    ' =====================
    ' CURRENT ADMIN USER
    ' =====================
    Dim currentAdminID As Integer = 0
    Dim currentAdminName As String = ""

    Public Sub SetUser(ByVal userID As Integer, ByVal fullName As String)
        currentAdminID = userID
        currentAdminName = fullName
    End Sub

    ' =====================
    ' FORM LOAD
    ' =====================
    Private Sub AdminPanel_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load
        Me.Text = "Admin Panel"
        Me.Size = New Size(1000, 620)
        Me.BackColor = Color.FromArgb(240, 240, 240)
        Me.StartPosition = FormStartPosition.CenterScreen
        ShowDashboard()
    End Sub

    ' =====================
    ' SIDEBAR BUTTONS
    ' =====================
    Private Sub btnDashboard_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDashboard.Click
        ShowDashboard()
    End Sub

    Private Sub btnInventory_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnInventory.Click
        ShowInventory()
    End Sub

    Private Sub btnSales_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnSales.Click
        ShowSales()
    End Sub

    Private Sub btnUsers_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUsers.Click
        ShowUsers()
    End Sub

    Private Sub btnExit_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnExit.Click
        Me.Close()
        LoginForm.Show()
    End Sub

    ' =====================
    ' CLEAR CONTENT PANEL
    ' =====================
    Private Sub ClearContent()
        pnlContent.Controls.Clear()
    End Sub

    ' =====================
    ' DASHBOARD
    ' =====================
    Private Sub ShowDashboard()
        ClearContent()

        Dim pnlHeader As New Panel
        pnlHeader.Size = New Size(815, 55)
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.BackColor = Color.FromArgb(44, 62, 80)
        pnlContent.Controls.Add(pnlHeader)

        Dim lblTitle As New Label
        lblTitle.Text = "DASHBOARD"
        lblTitle.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTitle.Location = New Point(15, 0)
        lblTitle.Size = New Size(400, 55)
        lblTitle.ForeColor = Color.White
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblTitle)

        Dim totalOrders As Integer = 0
        Dim totalRevenue As Decimal = 0
        Dim lowStock As Integer = 0

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()

                Dim cmd1 As New SqlCommand( _
                    "SELECT COUNT(*) FROM Orders WHERE Status = 'Completed'", conn)
                totalOrders = CInt(cmd1.ExecuteScalar())

                Dim cmd2 As New SqlCommand( _
                    "SELECT ISNULL(SUM(TotalAmount),0) FROM Orders " & _
                    "WHERE Status = 'Completed'", conn)
                totalRevenue = CDec(cmd2.ExecuteScalar())

                Dim cmd3 As New SqlCommand( _
                    "SELECT COUNT(*) FROM Inventory " & _
                    "WHERE StockQuantity <= LowStockAlert", conn)
                lowStock = CInt(cmd3.ExecuteScalar())
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try

        AddSummaryCard("TOTAL ORDERS", totalOrders.ToString(), _
            Color.FromArgb(52, 152, 219), _
            Color.FromArgb(41, 128, 185), New Point(15, 70))

        AddSummaryCard("TOTAL REVENUE", "P" & totalRevenue.ToString("0.00"), _
            Color.FromArgb(46, 204, 113), _
            Color.FromArgb(39, 174, 96), New Point(285, 70))

        AddSummaryCard("LOW STOCK", lowStock.ToString(), _
            Color.FromArgb(231, 76, 60), _
            Color.FromArgb(192, 57, 43), New Point(555, 70))

        Dim lblRecent As New Label
        lblRecent.Text = "Recent Orders"
        lblRecent.Font = New Font("Arial", 11, FontStyle.Bold)
        lblRecent.Location = New Point(15, 205)
        lblRecent.Size = New Size(200, 28)
        lblRecent.ForeColor = Color.FromArgb(44, 62, 80)
        pnlContent.Controls.Add(lblRecent)

        Dim dgv As New DataGridView
        dgv.Location = New Point(15, 238)
        dgv.Size = New Size(780, 330)
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.ReadOnly = True
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.Font = New Font("Arial", 9)
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Bold)
        dgv.EnableHeadersVisualStyles = False
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255)
        pnlContent.Controls.Add(dgv)

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "SELECT TOP 20 o.OrderID, u.FullName AS Cashier, " & _
                    "o.OrderDate, o.TotalAmount, o.Status " & _
                    "FROM Orders o " & _
                    "JOIN Users u ON o.CashierID = u.UserID " & _
                    "ORDER BY o.OrderDate DESC", conn)
                Dim da As New SqlDataAdapter(cmd)
                Dim dt As New DataTable
                da.Fill(dt)
                dgv.DataSource = dt
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' SUMMARY CARD
    ' =====================
    Private Sub AddSummaryCard(ByVal title As String, ByVal value As String, _
        ByVal topColor As Color, ByVal bottomColor As Color, _
        ByVal location As Point)

        Dim pnl As New Panel
        pnl.Size = New Size(240, 115)
        pnl.Location = location
        pnl.BackColor = topColor

        Dim pnlAccent As New Panel
        pnlAccent.Size = New Size(240, 6)
        pnlAccent.Location = New Point(0, 0)
        pnlAccent.BackColor = bottomColor
        pnl.Controls.Add(pnlAccent)

        Dim lblVal As New Label
        lblVal.Text = value
        lblVal.Font = New Font("Arial", 22, FontStyle.Bold)
        lblVal.ForeColor = Color.White
        lblVal.Location = New Point(15, 20)
        lblVal.Size = New Size(210, 50)
        lblVal.TextAlign = ContentAlignment.MiddleLeft
        pnl.Controls.Add(lblVal)

        Dim lblT As New Label
        lblT.Text = title
        lblT.Font = New Font("Arial", 8, FontStyle.Bold)
        lblT.ForeColor = Color.FromArgb(220, 240, 255)
        lblT.Location = New Point(15, 85)
        lblT.Size = New Size(210, 20)
        pnl.Controls.Add(lblT)

        pnlContent.Controls.Add(pnl)
    End Sub

    ' =====================
    ' INVENTORY MANAGEMENT (CRUD)
    ' =====================
    Private Sub ShowInventory()
        ClearContent()

        Dim pnlHeader As New Panel
        pnlHeader.Size = New Size(815, 55)
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.BackColor = Color.FromArgb(44, 62, 80)
        pnlContent.Controls.Add(pnlHeader)

        Dim lblTitle As New Label
        lblTitle.Text = "INVENTORY MANAGEMENT"
        lblTitle.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTitle.Location = New Point(15, 0)
        lblTitle.Size = New Size(400, 55)
        lblTitle.ForeColor = Color.White
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblTitle)

        Dim btnAdd As New Button
        btnAdd.Text = "ADD ITEM"
        btnAdd.Size = New Size(100, 30)
        btnAdd.Location = New Point(15, 65)
        btnAdd.BackColor = Color.FromArgb(39, 174, 96)
        btnAdd.ForeColor = Color.White
        btnAdd.FlatStyle = FlatStyle.Flat
        btnAdd.Font = New Font("Arial", 9, FontStyle.Bold)
        AddHandler btnAdd.Click, AddressOf BtnAddItem_Click
        pnlContent.Controls.Add(btnAdd)

        Dim btnRestock As New Button
        btnRestock.Text = "RESTOCK"
        btnRestock.Size = New Size(100, 30)
        btnRestock.Location = New Point(125, 65)
        btnRestock.BackColor = Color.FromArgb(41, 128, 185)
        btnRestock.ForeColor = Color.White
        btnRestock.FlatStyle = FlatStyle.Flat
        btnRestock.Font = New Font("Arial", 9, FontStyle.Bold)
        AddHandler btnRestock.Click, Sub(s, ev) RestockItem(dgvInventory)
        pnlContent.Controls.Add(btnRestock)

        Dim btnEditPrice As New Button
        btnEditPrice.Text = "EDIT PRICE"
        btnEditPrice.Size = New Size(100, 30)
        btnEditPrice.Location = New Point(235, 65)
        btnEditPrice.BackColor = Color.FromArgb(243, 156, 18)
        btnEditPrice.ForeColor = Color.White
        btnEditPrice.FlatStyle = FlatStyle.Flat
        btnEditPrice.Font = New Font("Arial", 9, FontStyle.Bold)
        AddHandler btnEditPrice.Click, Sub(s, ev) EditItemPrice(dgvInventory)
        pnlContent.Controls.Add(btnEditPrice)

        Dim btnDelete As New Button
        btnDelete.Text = "DELETE"
        btnDelete.Size = New Size(100, 30)
        btnDelete.Location = New Point(345, 65)
        btnDelete.BackColor = Color.FromArgb(192, 57, 43)
        btnDelete.ForeColor = Color.White
        btnDelete.FlatStyle = FlatStyle.Flat
        btnDelete.Font = New Font("Arial", 9, FontStyle.Bold)
        AddHandler btnDelete.Click, Sub(s, ev) DeleteItem(dgvInventory)
        pnlContent.Controls.Add(btnDelete)

        dgvInventory = New DataGridView
        dgvInventory.Name = "dgvInventory"
        dgvInventory.Location = New Point(15, 105)
        dgvInventory.Size = New Size(780, 450)
        dgvInventory.BackgroundColor = Color.White
        dgvInventory.BorderStyle = BorderStyle.None
        dgvInventory.RowHeadersVisible = False
        dgvInventory.AllowUserToAddRows = False
        dgvInventory.ReadOnly = True
        dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvInventory.Font = New Font("Arial", 9)
        dgvInventory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
        dgvInventory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvInventory.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Bold)
        dgvInventory.EnableHeadersVisualStyles = False
        dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255)
        pnlContent.Controls.Add(dgvInventory)

        LoadInventoryData(dgvInventory)
    End Sub

    ' =====================
    ' INVENTORY VARIABLE
    ' =====================
    Dim dgvInventory As DataGridView

    ' =====================
    ' LOAD INVENTORY DATA
    ' =====================
    Private Sub LoadInventoryData(ByVal dgv As DataGridView)
        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                ' FIX: Added WHERE mi.IsAvailable = 1 para hindi lumabas ang deleted items
                Dim cmd As New SqlCommand( _
                    "SELECT mi.ItemID, mi.ItemName, c.CategoryName, " & _
                    "mi.Price, i.StockQuantity, i.LowStockAlert, " & _
                    "CASE WHEN i.StockQuantity <= i.LowStockAlert " & _
                    "THEN 'LOW STOCK' ELSE 'OK' END AS StockStatus " & _
                    "FROM MenuItems mi " & _
                    "JOIN Categories c ON mi.CategoryID = c.CategoryID " & _
                    "LEFT JOIN Inventory i ON mi.ItemID = i.ItemID " & _
                    "WHERE mi.IsAvailable = 1 " & _
                    "ORDER BY c.CategoryName, mi.ItemName", conn)
                Dim da As New SqlDataAdapter(cmd)
                Dim dt As New DataTable
                da.Fill(dt)
                dgv.DataSource = dt

                For Each row As DataGridViewRow In dgv.Rows
                    If row.Cells("StockStatus").Value.ToString() = "LOW STOCK" Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200)
                        row.DefaultCellStyle.ForeColor = Color.DarkRed
                    End If
                Next
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' ADD ITEM
    ' =====================
    Private Sub BtnAddItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim itemName As String = InputBox("Item Name:", "Add Item")
        If itemName = "" Then Return

        Dim priceStr As String = InputBox("Price:", "Add Item")
        If priceStr = "" Then Return
        Dim price As Decimal
        If Not Decimal.TryParse(priceStr, price) Then
            MsgBox("Mali ang price!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim stockStr As String = InputBox("Initial Stock:", "Add Item")
        If stockStr = "" Then Return
        Dim stock As Integer
        If Not Integer.TryParse(stockStr, stock) Then
            MsgBox("Mali ang stock!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim categories As String = ""
        Dim catList As New List(Of Integer)
        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "SELECT CategoryID, CategoryName FROM Categories", conn)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    categories &= dr("CategoryID").ToString() & _
                                  " - " & dr("CategoryName").ToString() & vbCrLf
                    catList.Add(CInt(dr("CategoryID")))
                End While
                dr.Close()
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
            Return
        End Try

        Dim catStr As String = InputBox( _
            "Piliin ang CategoryID:" & vbCrLf & categories, "Add Item")
        If catStr = "" Then Return
        Dim catID As Integer
        If Not Integer.TryParse(catStr, catID) Then
            MsgBox("Mali ang CategoryID!", MsgBoxStyle.Exclamation)
            Return
        End If

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim trans As SqlTransaction = conn.BeginTransaction()
                Try
                    Dim cmdItem As New SqlCommand( _
                        "INSERT INTO MenuItems (CategoryID, ItemName, Price) " & _
                        "VALUES (@CatID, @Name, @Price); " & _
                        "SELECT SCOPE_IDENTITY();", conn, trans)
                    cmdItem.Parameters.AddWithValue("@CatID", catID)
                    cmdItem.Parameters.AddWithValue("@Name", itemName)
                    cmdItem.Parameters.AddWithValue("@Price", price)
                    Dim newItemID As Integer = CInt(cmdItem.ExecuteScalar())

                    Dim cmdInv As New SqlCommand( _
                        "INSERT INTO Inventory (ItemID, StockQuantity) " & _
                        "VALUES (@ItemID, @Stock)", conn, trans)
                    cmdInv.Parameters.AddWithValue("@ItemID", newItemID)
                    cmdInv.Parameters.AddWithValue("@Stock", stock)
                    cmdInv.ExecuteNonQuery()

                    trans.Commit()
                    MsgBox("Item added successfully!", MsgBoxStyle.Information)
                    LoadInventoryData(dgvInventory)
                Catch ex As Exception
                    trans.Rollback()
                    MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical)
                End Try
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' RESTOCK
    ' =====================
    Private Sub RestockItem(ByVal dgv As DataGridView)
        If dgv.SelectedRows.Count = 0 Then
            MsgBox("Pumili muna ng item!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim itemID As Integer = CInt(dgv.SelectedRows(0).Cells("ItemID").Value)
        Dim itemName As String = dgv.SelectedRows(0).Cells("ItemName").Value.ToString()
        Dim current As Integer = CInt(dgv.SelectedRows(0).Cells("StockQuantity").Value)

        Dim addStr As String = InputBox( _
            "Item: " & itemName & vbCrLf & _
            "Current Stock: " & current & vbCrLf & _
            "Magkano ang idadagdag?", "Restock")
        If addStr = "" Then Return

        Dim addQty As Integer
        If Not Integer.TryParse(addStr, addQty) OrElse addQty <= 0 Then
            MsgBox("Mali ang quantity!", MsgBoxStyle.Exclamation)
            Return
        End If

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "UPDATE Inventory SET StockQuantity = StockQuantity + @Qty, " & _
                    "LastUpdated = GETDATE() WHERE ItemID = @ItemID", conn)
                cmd.Parameters.AddWithValue("@Qty", addQty)
                cmd.Parameters.AddWithValue("@ItemID", itemID)
                cmd.ExecuteNonQuery()

                ' FIX: ChangedBy now uses actual admin ID instead of hardcoded 1
                Dim cmdLog As New SqlCommand( _
                    "INSERT INTO InventoryLog " & _
                    "(ItemID, ChangeType, QuantityChanged, Reason, ChangedBy) " & _
                    "VALUES (@ItemID, 'Restock', @Qty, 'Admin Restock', @AdminID)", conn)
                cmdLog.Parameters.AddWithValue("@ItemID", itemID)
                cmdLog.Parameters.AddWithValue("@Qty", addQty)
                cmdLog.Parameters.AddWithValue("@AdminID", currentAdminID)
                cmdLog.ExecuteNonQuery()
            End Using

            MsgBox("Restocked! +" & addQty & " units", MsgBoxStyle.Information)
            LoadInventoryData(dgvInventory)
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' EDIT PRICE
    ' =====================
    Private Sub EditItemPrice(ByVal dgv As DataGridView)
        If dgv.SelectedRows.Count = 0 Then
            MsgBox("Pumili muna ng item!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim itemID As Integer = CInt(dgv.SelectedRows(0).Cells("ItemID").Value)
        Dim itemName As String = dgv.SelectedRows(0).Cells("ItemName").Value.ToString()
        Dim currentPrice As String = dgv.SelectedRows(0).Cells("Price").Value.ToString()

        Dim newPrice As String = InputBox( _
            "Item: " & itemName & vbCrLf & _
            "Current Price: P" & currentPrice & vbCrLf & _
            "Bagong price:", "Edit Price", currentPrice)
        If newPrice = "" Then Return

        Dim price As Decimal
        If Not Decimal.TryParse(newPrice, price) OrElse price < 0 Then
            MsgBox("Mali ang price!", MsgBoxStyle.Exclamation)
            Return
        End If

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "UPDATE MenuItems SET Price = @Price WHERE ItemID = @ItemID", conn)
                cmd.Parameters.AddWithValue("@Price", price)
                cmd.Parameters.AddWithValue("@ItemID", itemID)
                cmd.ExecuteNonQuery()
            End Using
            MsgBox("Price updated!", MsgBoxStyle.Information)
            LoadInventoryData(dgvInventory)
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' DELETE ITEM
    ' =====================
    Private Sub DeleteItem(ByVal dgv As DataGridView)
        If dgv.SelectedRows.Count = 0 Then
            MsgBox("Pumili muna ng item!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim itemID As Integer = CInt(dgv.SelectedRows(0).Cells("ItemID").Value)
        Dim itemName As String = dgv.SelectedRows(0).Cells("ItemName").Value.ToString()

        Dim confirm As MsgBoxResult = MsgBox( _
            "I-delete ang '" & itemName & "'?", _
            MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Confirm Delete")
        If confirm <> MsgBoxResult.Yes Then Return

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "UPDATE MenuItems SET IsAvailable = 0 " & _
                    "WHERE ItemID = @ItemID", conn)
                cmd.Parameters.AddWithValue("@ItemID", itemID)
                cmd.ExecuteNonQuery()
            End Using
            MsgBox("Item deleted!", MsgBoxStyle.Information)
            LoadInventoryData(dgvInventory)
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' SALES REPORT
    ' =====================
    Private Sub ShowSales()
        ClearContent()

        Dim pnlHeader As New Panel
        pnlHeader.Size = New Size(815, 55)
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.BackColor = Color.FromArgb(44, 62, 80)
        pnlContent.Controls.Add(pnlHeader)

        Dim lblTitle As New Label
        lblTitle.Text = "SALES REPORT"
        lblTitle.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTitle.Location = New Point(15, 0)
        lblTitle.Size = New Size(400, 55)
        lblTitle.ForeColor = Color.White
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblTitle)

        Dim dgv As New DataGridView
        dgv.Location = New Point(15, 65)
        dgv.Size = New Size(780, 490)
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.ReadOnly = True
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.Font = New Font("Arial", 9)
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Bold)
        dgv.EnableHeadersVisualStyles = False
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255)
        pnlContent.Controls.Add(dgv)

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "SELECT CAST(o.OrderDate AS DATE) AS SaleDate, " & _
                    "COUNT(DISTINCT o.OrderID) AS TotalOrders, " & _
                    "SUM(o.TotalAmount) AS TotalRevenue " & _
                    "FROM Orders o " & _
                    "WHERE o.Status = 'Completed' " & _
                    "GROUP BY CAST(o.OrderDate AS DATE) " & _
                    "ORDER BY SaleDate DESC", conn)
                Dim da As New SqlDataAdapter(cmd)
                Dim dt As New DataTable
                da.Fill(dt)
                dgv.DataSource = dt
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' USERS
    ' =====================
    Private Sub ShowUsers()
        ClearContent()

        Dim pnlHeader As New Panel
        pnlHeader.Size = New Size(815, 55)
        pnlHeader.Location = New Point(0, 0)
        pnlHeader.BackColor = Color.FromArgb(44, 62, 80)
        pnlContent.Controls.Add(pnlHeader)

        Dim lblTitle As New Label
        lblTitle.Text = "USER MANAGEMENT"
        lblTitle.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTitle.Location = New Point(15, 0)
        lblTitle.Size = New Size(400, 55)
        lblTitle.ForeColor = Color.White
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblTitle)

        Dim dgv As New DataGridView
        dgv.Location = New Point(15, 65)
        dgv.Size = New Size(780, 490)
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.ReadOnly = True
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.Font = New Font("Arial", 9)
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Bold)
        dgv.EnableHeadersVisualStyles = False
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255)
        pnlContent.Controls.Add(dgv)

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand( _
                    "SELECT UserID, FullName, Username, Role, " & _
                    "CASE WHEN IsActive = 1 THEN 'Active' " & _
                    "ELSE 'Inactive' END AS Status, " & _
                    "CreatedAt FROM Users " & _
                    "ORDER BY Role, FullName", conn)
                Dim da As New SqlDataAdapter(cmd)
                Dim dt As New DataTable
                da.Fill(dt)
                dgv.DataSource = dt
            End Using
        Catch ex As Exception
            MsgBox("DB Error: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

End Class