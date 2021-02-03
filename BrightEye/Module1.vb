Imports dnlib.DotNet
Module Module1

    Sub Main(ByVal args As String())
        Console.Title = "BrightEye v1.0"
        Console.WriteLine("BrightEye v1.0 by misonothx - Decrypter & Unpacker for DarkEye")
        Console.WriteLine()
        If args.Count = 0 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Please set an input file!")
            Console.ReadKey()
            End
        End If
        Dim patchedApp As dnlib.DotNet.ModuleDef = dnlib.DotNet.ModuleDefMD.Load(args(0))
        Dim strings As New List(Of String)
        For x = 0 To patchedApp.EntryPoint.Body.Instructions.Count - 1
            Dim currentInstruction As Emit.Instruction = patchedApp.EntryPoint.Body.Instructions(x)
            If currentInstruction.OpCode.ToString = "ldstr" Then
                strings.Add(currentInstruction.Operand.ToString)
            End If
        Next
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.WriteLine("Encryption Key: " & strings(1))
        Console.WriteLine()
        If Not IO.Directory.Exists("BrightEye") Then
            IO.Directory.CreateDirectory("BrightEye")
        End If
        IO.Directory.CreateDirectory("BrightEye\" & IO.Path.GetFileNameWithoutExtension(args(0)))
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("Extracting """ & strings(2) & ".bin""...")
        Try
            IO.File.WriteAllBytes("BrightEye\" & IO.Path.GetFileNameWithoutExtension(args(0)) & "\" & strings(2) & ".bin", RC4(Text.Encoding.Default.GetBytes(strings(1)), StringToByteArray((XorEnc(strings(0))))))
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Failed to extract """ & strings(2) & ".bin""! (exception: " & ex.Message & ")")
            Console.ReadKey()
            End
        End Try
        Console.ForegroundColor = ConsoleColor.Green
        Console.Write("Successfully extracted """ & strings(2) & ".bin""!")
        Console.ReadKey()
        End
    End Sub
    Private Function StringToByteArray(ByVal hex As String) As Byte()
        Dim length As Integer = hex.Length
        Dim array As Byte() = New Byte(length / 2 - 1) {}
        For i As Integer = 0 To length - 1 Step 2
            array(i / 2) = Convert.ToByte(hex.Substring(i, 2), 16)
        Next
        Return array
    End Function
    Private Function XorEnc(ByVal target As String) As String
        Dim text As String = ""
        For i As Integer = 0 To target.Length - 1
            Dim c As Char = ChrW(AscW(target(i)) Xor AscW("{"))
            text += c
        Next
        Return text
    End Function
    Private Function RC4(ByVal pwd As Byte(), ByVal data As Byte()) As Byte()
        Dim array As Integer() = New Integer(255) {}
        Dim array2 As Integer() = New Integer(255) {}
        Dim array3 As Byte() = New Byte(data.Length - 1) {}
        Dim i As Integer
        i = 0
        While i < 256
            array(i) = CInt(pwd(i Mod pwd.Length))
            array2(i) = i
            i += 1
        End While
        Dim num As Integer = 0
        i = num
        Dim num2 As Integer = num
        While i < 256
            num2 = (num2 + array2(i) + array(i)) Mod 256
            Dim num3 As Integer = array2(i)
            array2(i) = array2(num2)
            array2(num2) = num3
            i += 1
        End While
        Dim num4 As Integer = 0
        i = num4
        num2 = num4
        Dim num5 As Integer = num4
        While i < data.Length
            num5 += 1
            num5 = num5 Mod 256
            num2 += array2(num5)
            num2 = num2 Mod 256
            Dim num3 As Integer = array2(num5)
            array2(num5) = array2(num2)
            array2(num2) = num3
            Dim num6 As Integer = array2((array2(num5) + array2(num2)) Mod 256)
            array3(i) = CByte((CInt(data(i)) Xor num6))
            i += 1
        End While
        Return array3
    End Function
End Module
