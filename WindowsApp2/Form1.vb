Public Class Form1
    Private lst As ListBox
    Public Sub New()

        InitializeComponent()

        lst = ListBox1
        lst.DrawMode = DrawMode.OwnerDrawFixed

        lst.Items.Add("70")
        lst.Items.Add("20")
        lst.Items.Add("120")

    End Sub

    Private Sub listbox_colorset(sender As Object, e As DrawItemEventArgs) Handles ListBox1.DrawItem

        e.DrawBackground()

        Dim myBrush As Brush = Brushes.White

        ' listbox의 값을 정수형으로 저장한다.
        Dim sayi As Integer = Convert.ToInt32(TryCast(sender, ListBox).Items(e.Index).ToString)

        ' listbox의 아이템이 100 이상이면 빨간색, 이하면 초록색으로 브러쉬를 지정한다.
        If sayi > 100 Then

            myBrush = Brushes.Red
        Else
            myBrush = Brushes.Green

        End If

        ' listbox의 값을 기준으로 brush를 그린다.
        e.Graphics.DrawString(
            TryCast(sender, ListBox).Items(e.Index).ToString,
            e.Font,
            myBrush,
            e.Bounds,
            StringFormat.GenericDefault)

        e.DrawFocusRectangle()

    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub


End Class
