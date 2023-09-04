Imports NUnit.Framework

Namespace TestPDFA

    Public Class Tests

        <SetUp>
        Public Sub Setup()
        End Sub

        <Test>
        Public Sub Test1()
            Dim conv As New CConvToPDFA
            conv.ConvertFile(DynaPDF.TConformanceType.ctPDFA_3b, "C:\unittest\testattachments\testPW.pdf", "out.pdf")
        End Sub

    End Class

End Namespace