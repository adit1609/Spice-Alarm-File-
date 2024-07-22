Imports ActUtlTypeLib

Public Class Form1
    Dim plc As New ActUtlType
    Dim isUpdating As Boolean = False ' Flag to prevent event re-entry
    Dim lastReadValue As Single = Single.NaN ' Variable to store the last read value

    Public Function ConvertFloatToWord(ByVal value As Single) As Integer()
        Dim floatBytes As Byte() = BitConverter.GetBytes(value)
        Dim lowWord As Integer = BitConverter.ToInt16(floatBytes, 0)
        Dim highWord As Integer = BitConverter.ToInt16(floatBytes, 2)
        Return {lowWord, highWord}
    End Function

    Public Function ConvertWordToFloat(ByVal register As Integer()) As Single
        Dim bytes(3) As Byte
        Dim lowWordBytes() As Byte = BitConverter.GetBytes(register(0))
        Dim highWordBytes() As Byte = BitConverter.GetBytes(register(1))

        Array.Copy(lowWordBytes, 0, bytes, 0, 2)
        Array.Copy(highWordBytes, 0, bytes, 2, 2)

        Return BitConverter.ToSingle(bytes, 0)
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        plc.ActLogicalStationNumber = 1
        plc.Open()

        ' Start the timer to periodically read the PLC registers
        Dim timer As New Timer()
        AddHandler timer.Tick, AddressOf Timer_Tick
        timer.Interval = 1000 ' 1 second interval
        timer.Start()
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        ' Read the values from the PLC registers
        Dim readWords(1) As Integer
        plc.GetDevice("D2002", readWords(0))
        plc.GetDevice("D2003", readWords(1))

        ' Convert the words to a float value
        Dim floatValue As Single = ConvertWordToFloat(readWords)

        ' Update the text box if the value has changed
        If Not floatValue.Equals(lastReadValue) Then
            isUpdating = True
            Guna2TextBox1.Text = floatValue.ToString("F6")
            isUpdating = False
            lastReadValue = floatValue
        End If
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        If isUpdating Then
            Return
        End If

        Dim floatValue As Single
        If Single.TryParse(Guna2TextBox1.Text, floatValue) Then
            ' Convert the float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(floatValue)

            ' Write the integers to the PLC registers
            plc.SetDevice("D2002", words(0))
            plc.SetDevice("D2003", words(1))

            ' Update the last read value to prevent unnecessary updates
            lastReadValue = floatValue
        Else
            ' Handle invalid input if needed
            'MessageBox.Show("Invalid input. Please enter a valid float value.")
        End If
    End Sub
End Class
