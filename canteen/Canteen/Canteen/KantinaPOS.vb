Imports System.Data.SqlClient
Imports System.Linq

Public Class KantinaPOS

    ' =====================
    ' VARIABLES
    ' =====================
    Dim currentCashierID As Integer = 0       ' FIX: was hardcoded to 2
    Dim currentCashierName As String = ""     ' FIX: now actually used

    Public Sub SetUser(ByVal userID As Integer, ByVal fullName As String)
        currentCashierID = userID
        currentCashierName = fullName         ' FIX: set the name too
        Me.Text = "KANTINA POS v1.0  |  Cashier: " & fullName
    End Sub

    ' FIX: Removed duplicate local ConnStr — gumagamit na ng global sa DbConnection.vb

    Structure CartItem
        Dim ItemID As Integer
        Dim ItemName As String
        Dim Price As Decimal
        Dim Quantity As Integer
    End Structure

    Dim cart As New List(Of CartItem)

    ' =====================
    ' FORM LOAD
    ' =====================
    Private Sub KantinaPOS_Load(ByVal sender As Object, ByVal e As EventArgs) _
    Handles MyBase.Load
        Me.Text = "KANTINA POS v1.0"
        Me.Size = New Size(950, 620)
        Me.BackColor = Color.FromArgb(240, 240, 240)
        LoadCategories()
        LoadMenuByCategory(0)
        RefreshCart()
    End Sub

    ' =====================
    ' LOAD CATEGORIES
    ' =====================
    Private Sub LoadCategories()
        flpCategories.Controls.Clear()

        Dim btnAll As New Button
        btnAll.Text = "[Lahat]"
        btnAll.Size = New Size(100, 40)
        btnAll.BackColor = Color.Gray
        btnAll.ForeColor = Color.White
        btnAll.Font = New Font("Arial", 9, FontStyle.Bold)
        btnAll.FlatStyle = FlatStyle.Flat
        btnAll.Tag = 0
        AddHandler btnAll.Click, AddressOf CategoryBtn_Click
        flpCategories.Controls.Add(btnAll)

        Dim colors() As Color = {
            Color.FromArgb(192, 57, 43),
            Color.FromArgb(39, 174, 96),
            Color.FromArgb(41, 128, 185),
            Color.FromArgb(243, 156, 18)
        }

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand(
                    "SELECT CategoryID, CategoryName FROM Categories", conn)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                Dim i As Integer = 0

                While dr.Read()
                    Dim btn As New Button
                    btn.Text = "[" & dr("CategoryName").ToString() & "]"
                    btn.Size = New Size(120, 40)
                    btn.BackColor = colors(i Mod colors.Length)
                    btn.ForeColor = Color.White
                    btn.Font = New Font("Arial", 9, FontStyle.Bold)
                    btn.FlatStyle = FlatStyle.Flat
                    btn.Tag = CInt(dr("CategoryID"))
                    AddHandler btn.Click, AddressOf CategoryBtn_Click
                    flpCategories.Controls.Add(btn)
                    i += 1
                End While

                dr.Close()
            End Using
        Catch ex As Exception
            MsgBox("DB Error (Categories): " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' CATEGORY BUTTON CLICK
    ' =====================
    Private Sub CategoryBtn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        LoadMenuByCategory(CInt(btn.Tag))
    End Sub

    ' =====================
    ' LOAD MENU ITEMS
    ' =====================
    Private Sub LoadMenuByCategory(ByVal categoryID As Integer)
        flpMenuItems.Controls.Clear()

        Dim query As String
        If categoryID = 0 Then
            query = "SELECT mi.ItemID, mi.ItemName, mi.Price, i.StockQuantity " & _
                    "FROM MenuItems mi " & _
                    "LEFT JOIN Inventory i ON mi.ItemID = i.ItemID " & _
                    "WHERE mi.IsAvailable = 1"
        Else
            ' FIX: parameterized query para iwas SQL injection
            query = "SELECT mi.ItemID, mi.ItemName, mi.Price, i.StockQuantity " & _
                    "FROM MenuItems mi " & _
                    "LEFT JOIN Inventory i ON mi.ItemID = i.ItemID " & _
                    "WHERE mi.IsAvailable = 1 AND mi.CategoryID = @CatID"
        End If

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim cmd As New SqlCommand(query, conn)
                If categoryID <> 0 Then
                    cmd.Parameters.AddWithValue("@CatID", categoryID)
                End If
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                While dr.Read()
                    Dim itemID As Integer = CInt(dr("ItemID"))
                    Dim itemName As String = dr("ItemName").ToString()
                    Dim price As Decimal = CDec(dr("Price"))
                    Dim stock As Integer = 0
                    If Not IsDBNull(dr("StockQuantity")) Then
                        stock = CInt(dr("StockQuantity"))
                    End If

                    Dim pnl As New Panel
                    pnl.Size = New Size(145, 115)
                    pnl.BackColor = Color.White
                    pnl.Margin = New Padding(5)

                    Dim lblName As New Label
                    lblName.Text = itemName
                    lblName.Font = New Font("Arial", 8, FontStyle.Bold)
                    lblName.Size = New Size(135, 35)
                    lblName.Location = New Point(5, 5)
                    lblName.TextAlign = ContentAlignment.MiddleCenter

                    Dim lblPrice As New Label
                    lblPrice.Text = "P" & price.ToString("0.00")
                    lblPrice.Font = New Font("Arial", 9, FontStyle.Bold)
                    lblPrice.ForeColor = Color.DarkGreen
                    lblPrice.Size = New Size(135, 20)
                    lblPrice.Location = New Point(5, 42)
                    lblPrice.TextAlign = ContentAlignment.MiddleCenter

                    Dim lblStock As New Label
                    lblStock.Text = "Stock: " & stock
                    lblStock.Font = New Font("Arial", 7)
                    If stock <= 10 Then
                        lblStock.ForeColor = Color.Red
                    Else
                        lblStock.ForeColor = Color.Gray
                    End If
                    lblStock.Size = New Size(135, 15)
                    lblStock.Location = New Point(5, 62)
                    lblStock.TextAlign = ContentAlignment.MiddleCenter

                    Dim btnAdd As New Button
                    btnAdd.Text = "+"
                    btnAdd.Size = New Size(55, 28)
                    btnAdd.Location = New Point(5, 80)
                    btnAdd.BackColor = Color.FromArgb(39, 174, 96)
                    btnAdd.ForeColor = Color.White
                    btnAdd.Font = New Font("Arial", 10, FontStyle.Bold)
                    btnAdd.FlatStyle = FlatStyle.Flat
                    btnAdd.Tag = New Object() {itemID, itemName, price}
                    AddHandler btnAdd.Click, AddressOf BtnAdd_Click

                    Dim btnMinus As New Button
                    btnMinus.Text = "-"
                    btnMinus.Size = New Size(55, 28)
                    btnMinus.Location = New Point(80, 80)
                    btnMinus.BackColor = Color.FromArgb(192, 57, 43)
                    btnMinus.ForeColor = Color.White
                    btnMinus.Font = New Font("Arial", 10, FontStyle.Bold)
                    btnMinus.FlatStyle = FlatStyle.Flat
                    btnMinus.Tag = New Object() {itemID, itemName, price}
                    AddHandler btnMinus.Click, AddressOf BtnMinus_Click

                    pnl.Controls.AddRange(New Control() {lblName, lblPrice, lblStock, btnAdd, btnMinus})
                    flpMenuItems.Controls.Add(pnl)
                End While

                dr.Close()
            End Using
        Catch ex As Exception
            MsgBox("DB Error (Menu): " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' ADD TO CART
    ' =====================
    Private Sub BtnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim info() As Object = CType(btn.Tag, Object())
        Dim itemID As Integer = CInt(info(0))
        Dim itemName As String = info(1).ToString()
        Dim price As Decimal = CDec(info(2))

        Dim idx As Integer = -1
        Dim x As Integer = 0
        For Each c As CartItem In cart
            If c.ItemID = itemID Then
                idx = x
                Exit For
            End If
            x += 1
        Next

        If idx >= 0 Then
            Dim updated As CartItem = cart(idx)
            updated.Quantity += 1
            cart(idx) = updated
        Else
            Dim newItem As New CartItem
            newItem.ItemID = itemID
            newItem.ItemName = itemName
            newItem.Price = price
            newItem.Quantity = 1
            cart.Add(newItem)
        End If

        RefreshCart()
    End Sub

    ' =====================
    ' MINUS FROM CART
    ' =====================
    Private Sub BtnMinus_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim info() As Object = CType(btn.Tag, Object())
        Dim itemID As Integer = CInt(info(0))

        Dim idx As Integer = -1
        Dim x As Integer = 0
        For Each c As CartItem In cart
            If c.ItemID = itemID Then
                idx = x
                Exit For
            End If
            x += 1
        Next

        If idx >= 0 Then
            Dim updated As CartItem = cart(idx)
            If updated.Quantity > 1 Then
                updated.Quantity -= 1
                cart(idx) = updated
            Else
                cart.RemoveAt(idx)
            End If
        End If

        RefreshCart()
    End Sub

    ' =====================
    ' REFRESH CART
    ' =====================
    Private Sub RefreshCart()
        lstCart.Items.Clear()
        Dim total As Decimal = 0

        For Each item As CartItem In cart
            lstCart.Items.Add(
                item.ItemName & " (P" & item.Price & ")  x" & item.Quantity & _
                "  = P" & (item.Price * item.Quantity).ToString("0.00"))
            total += item.Price * item.Quantity
        Next

        lblTotal.Text = "KABUUANG HALAGA: P" & total.ToString("0.00")
    End Sub

    ' =====================
    ' SINGILIN
    ' =====================
    Private Sub btnSingilin_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnSingilin.Click

        If cart.Count = 0 Then
            MsgBox("Walang laman ang order!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim total As Decimal = 0
        For Each item As CartItem In cart
            total += item.Price * item.Quantity
        Next

        Dim paid As String = InputBox(
            "Kabuuan: P" & total.ToString("0.00") & vbCrLf & "Magkano ang ibinayad?",
            "Bayad", total.ToString("0.00"))

        If paid = "" Then Return

        Dim amountPaid As Decimal
        If Not Decimal.TryParse(paid, amountPaid) OrElse amountPaid < total Then
            MsgBox("Hindi sapat ang bayad!", MsgBoxStyle.Exclamation)
            Return
        End If

        Dim change As Decimal = amountPaid - total

        Try
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Dim trans As SqlTransaction = conn.BeginTransaction()

                Try
                    ' FIX: Status = 'Pending' para makita ng Kitchen Display
                    Dim cmdOrder As New SqlCommand(
                        "INSERT INTO Orders (CashierID, TotalAmount, AmountPaid, Change, Status) " & _
                        "VALUES (@CashierID, @Total, @Paid, @Change, 'Pending'); " & _
                        "SELECT SCOPE_IDENTITY();", conn, trans)
                    cmdOrder.Parameters.AddWithValue("@CashierID", currentCashierID)
                    cmdOrder.Parameters.AddWithValue("@Total", total)
                    cmdOrder.Parameters.AddWithValue("@Paid", amountPaid)
                    cmdOrder.Parameters.AddWithValue("@Change", change)
                    Dim orderID As Integer = CInt(cmdOrder.ExecuteScalar())

                    For Each item As CartItem In cart
                        Dim cmdDetail As New SqlCommand(
                            "INSERT INTO OrderDetails (OrderID, ItemID, Quantity, UnitPrice) " & _
                            "VALUES (@OrderID, @ItemID, @Qty, @Price)", conn, trans)
                        cmdDetail.Parameters.AddWithValue("@OrderID", orderID)
                        cmdDetail.Parameters.AddWithValue("@ItemID", item.ItemID)
                        cmdDetail.Parameters.AddWithValue("@Qty", item.Quantity)
                        cmdDetail.Parameters.AddWithValue("@Price", item.Price)
                        cmdDetail.ExecuteNonQuery()

                        Dim cmdInv As New SqlCommand(
                            "UPDATE Inventory SET StockQuantity = StockQuantity - @Qty, " & _
                            "LastUpdated = GETDATE() WHERE ItemID = @ItemID", conn, trans)
                        cmdInv.Parameters.AddWithValue("@Qty", item.Quantity)
                        cmdInv.Parameters.AddWithValue("@ItemID", item.ItemID)
                        cmdInv.ExecuteNonQuery()

                        Dim cmdLog As New SqlCommand(
                            "INSERT INTO InventoryLog " & _
                            "(ItemID, ChangeType, QuantityChanged, Reason, ChangedBy) " & _
                            "VALUES (@ItemID, 'Deduction', @Qty, " & _
                            "'Sale - Order #' + CAST(@OrderID AS VARCHAR), @CashierID)",
                            conn, trans)
                        cmdLog.Parameters.AddWithValue("@ItemID", item.ItemID)
                        cmdLog.Parameters.AddWithValue("@Qty", item.Quantity)
                        cmdLog.Parameters.AddWithValue("@OrderID", orderID)
                        cmdLog.Parameters.AddWithValue("@CashierID", currentCashierID)
                        cmdLog.ExecuteNonQuery()
                    Next

                    trans.Commit()

                    MsgBox("ORDER COMPLETE!" & vbCrLf & vbCrLf & _
                           "Kabuuan:  P" & total.ToString("0.00") & vbCrLf & _
                           "Ibinayad: P" & amountPaid.ToString("0.00") & vbCrLf & _
                           "Sukli:    P" & change.ToString("0.00") & vbCrLf & vbCrLf & _
                           "Salamat po!", MsgBoxStyle.Information, "Bayad na!")

                    cart.Clear()
                    RefreshCart()
                    LoadMenuByCategory(0)

                Catch ex As Exception
                    trans.Rollback()
                    MsgBox("Error sa transaction: " & ex.Message, MsgBoxStyle.Critical)
                End Try
            End Using

        Catch ex As Exception
            MsgBox("Hindi makakonekta sa DB: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' =====================
    ' BURAHIN
    ' =====================
    Private Sub btnBurahin_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBurahin.Click

        If lstCart.SelectedIndex < 0 Then
            MsgBox("Pumili muna ng item sa listahan.", MsgBoxStyle.Information)
            Return
        End If

        cart.RemoveAt(lstCart.SelectedIndex)
        RefreshCart()
    End Sub

    Private Sub flpCategories_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles flpCategories.Paint

    End Sub

    Private Sub flpMenuItems_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles flpMenuItems.Paint

    End Sub

    Private Sub lblKasalukuyan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblKasalukuyan.Click

    End Sub

End Class