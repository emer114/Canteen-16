<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KantinaPOS
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.flpCategories = New System.Windows.Forms.FlowLayoutPanel()
        Me.flpMenuItems = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.lstCart = New System.Windows.Forms.ListBox()
        Me.btnSingilin = New System.Windows.Forms.Button()
        Me.btnBurahin = New System.Windows.Forms.Button()
        Me.lblKasalukuyan = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'flpCategories
        '
        Me.flpCategories.BackColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(97, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.flpCategories.Dock = System.Windows.Forms.DockStyle.Top
        Me.flpCategories.Location = New System.Drawing.Point(0, 0)
        Me.flpCategories.Name = "flpCategories"
        Me.flpCategories.Size = New System.Drawing.Size(934, 55)
        Me.flpCategories.TabIndex = 0
        '
        'flpMenuItems
        '
        Me.flpMenuItems.AutoScroll = True
        Me.flpMenuItems.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.flpMenuItems.Location = New System.Drawing.Point(0, 55)
        Me.flpMenuItems.Name = "flpMenuItems"
        Me.flpMenuItems.Size = New System.Drawing.Size(600, 510)
        Me.flpMenuItems.TabIndex = 1
        '
        'lblTotal
        '
        Me.lblTotal.AutoSize = True
        Me.lblTotal.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotal.ForeColor = System.Drawing.Color.DarkGreen
        Me.lblTotal.Location = New System.Drawing.Point(615, 440)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(202, 18)
        Me.lblTotal.TabIndex = 2
        Me.lblTotal.Text = "KABUUANG HALAGA: P0.00"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lstCart
        '
        Me.lstCart.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.lstCart.FormattingEnabled = True
        Me.lstCart.ItemHeight = 15
        Me.lstCart.Location = New System.Drawing.Point(615, 100)
        Me.lstCart.Name = "lstCart"
        Me.lstCart.Size = New System.Drawing.Size(300, 319)
        Me.lstCart.TabIndex = 3
        '
        'btnSingilin
        '
        Me.btnSingilin.BackColor = System.Drawing.Color.FromArgb(CType(CType(41, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.btnSingilin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSingilin.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSingilin.ForeColor = System.Drawing.Color.White
        Me.btnSingilin.Location = New System.Drawing.Point(615, 490)
        Me.btnSingilin.Name = "btnSingilin"
        Me.btnSingilin.Size = New System.Drawing.Size(149, 45)
        Me.btnSingilin.TabIndex = 4
        Me.btnSingilin.Text = "SINGILIN (Enter)"
        Me.btnSingilin.UseVisualStyleBackColor = False
        '
        'btnBurahin
        '
        Me.btnBurahin.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(57, Byte), Integer), CType(CType(43, Byte), Integer))
        Me.btnBurahin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBurahin.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBurahin.ForeColor = System.Drawing.Color.White
        Me.btnBurahin.Location = New System.Drawing.Point(770, 490)
        Me.btnBurahin.Name = "btnBurahin"
        Me.btnBurahin.Size = New System.Drawing.Size(140, 45)
        Me.btnBurahin.TabIndex = 5
        Me.btnBurahin.Text = "Burahin (Del)"
        Me.btnBurahin.UseVisualStyleBackColor = False
        '
        'lblKasalukuyan
        '
        Me.lblKasalukuyan.BackColor = System.Drawing.Color.FromArgb(CType(CType(41, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.lblKasalukuyan.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblKasalukuyan.ForeColor = System.Drawing.Color.White
        Me.lblKasalukuyan.Location = New System.Drawing.Point(615, 65)
        Me.lblKasalukuyan.Name = "lblKasalukuyan"
        Me.lblKasalukuyan.Size = New System.Drawing.Size(300, 30)
        Me.lblKasalukuyan.TabIndex = 6
        Me.lblKasalukuyan.Text = "Kasalukuyang Order"
        Me.lblKasalukuyan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KantinaPOS
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(934, 581)
        Me.Controls.Add(Me.lblKasalukuyan)
        Me.Controls.Add(Me.btnBurahin)
        Me.Controls.Add(Me.btnSingilin)
        Me.Controls.Add(Me.lstCart)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.flpMenuItems)
        Me.Controls.Add(Me.flpCategories)
        Me.Name = "KantinaPOS"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KANTINA POS"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents flpCategories As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents flpMenuItems As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents lstCart As System.Windows.Forms.ListBox
    Friend WithEvents btnSingilin As System.Windows.Forms.Button
    Friend WithEvents btnBurahin As System.Windows.Forms.Button
    Friend WithEvents lblKasalukuyan As System.Windows.Forms.Label

End Class
