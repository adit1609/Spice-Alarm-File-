<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form5
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
        Me.Guna2TextBox1 = New Guna.UI2.WinForms.Guna2TextBox()
        Me.pnwrong = New Guna.UI2.WinForms.Guna2Panel()
        Me.pnright = New Guna.UI2.WinForms.Guna2Panel()
        Me.SuspendLayout()
        '
        'Guna2TextBox1
        '
        Me.Guna2TextBox1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.Guna2TextBox1.DefaultText = ""
        Me.Guna2TextBox1.DisabledState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.Guna2TextBox1.DisabledState.FillColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.Guna2TextBox1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.Guna2TextBox1.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.Guna2TextBox1.FocusedState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Guna2TextBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Guna2TextBox1.HoverState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Guna2TextBox1.Location = New System.Drawing.Point(173, 130)
        Me.Guna2TextBox1.Name = "Guna2TextBox1"
        Me.Guna2TextBox1.PasswordChar = Global.Microsoft.VisualBasic.ChrW(0)
        Me.Guna2TextBox1.PlaceholderText = ""
        Me.Guna2TextBox1.SelectedText = ""
        Me.Guna2TextBox1.Size = New System.Drawing.Size(149, 103)
        Me.Guna2TextBox1.TabIndex = 0
        '
        'pnwrong
        '
        Me.pnwrong.BackgroundImage = Global.Number_System.My.Resources.Resources.icons8_error_50
        Me.pnwrong.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pnwrong.Location = New System.Drawing.Point(328, 164)
        Me.pnwrong.Name = "pnwrong"
        Me.pnwrong.Size = New System.Drawing.Size(29, 32)
        Me.pnwrong.TabIndex = 2
        '
        'pnright
        '
        Me.pnright.BackgroundImage = Global.Number_System.My.Resources.Resources.icons8_tick_481
        Me.pnright.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pnright.Location = New System.Drawing.Point(328, 167)
        Me.pnright.Name = "pnright"
        Me.pnright.Size = New System.Drawing.Size(29, 32)
        Me.pnright.TabIndex = 1
        '
        'Form5
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(772, 450)
        Me.Controls.Add(Me.pnwrong)
        Me.Controls.Add(Me.pnright)
        Me.Controls.Add(Me.Guna2TextBox1)
        Me.Name = "Form5"
        Me.Text = "Form5"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Guna2TextBox1 As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents pnright As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents pnwrong As Guna.UI2.WinForms.Guna2Panel
End Class
