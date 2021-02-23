Public Class Form1
    Public Sub New()

        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하세요.

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim chkLists As List(Of Control) = New List(Of Control) From {
        TextBox1, TextBox2, TextBox3, TextBox4
        }

        Dim cnt As Integer = chkLists.Where(Function(x) x.Text = "").Count

        If (cnt > 0) Then
            MessageBox.Show("필수 정보가 모두 입력되지 않았습니다.", "악마성")
            Exit Sub
        Else
            MessageBox.Show("모두 입력 되었습니다!", "악마성")
        End If

    End Sub


    Private Sub Picture_Test()
        Dim picbox = New PictureBox

        With picbox
            .BorderStyle = BorderStyle.FixedSingle
            .Top = 100
            .Left = 100
            .SizeMode = PictureBoxSizeMode.AutoSize
            .Size = New Drawing.Size(231, 223)
            .Name = "test"
            .Image = Image.FromFile("C:\\Guide.gif")
        End With

        Controls.Add(picbox)
        picbox.BringToFront()

    End Sub

End Class
