.SUFFIXES: .cs .java
##
## -------------------------------------------------------
## makefile
##
## MsmqJava
##
## used to build the MsmqJava library.
## See http://MsmqJava.codeplex.com
##
## Time-stamp: <2010-March-28 16:50:41>
##
##-------------------------------------------------------
##

# ============================================
# C# settings
NETDIR=c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727
CSC=$(NETDIR)\csc.exe

# ============================================
# Java settings
JAVADIR=c:\sunjdk
#JAVADIR=c:\ibmjdk5
JAVAC=$(JAVADIR)\bin\javac.exe


BINDIR=bin
OBJDIR=obj


# ============================================
# build targets
#
default: $(BINDIR)\TestClient.exe  $(BINDIR)\EnumQueues.exe  $(BINDIR)\TestClient.class


$(BINDIR)\EnumQueues.exe: EnumQueues.cs
        if not exist $(BINDIR)\. mkdir $(BINDIR)
        $(CSC) $(CSCOPTIONS) /target:exe   /out:$(BINDIR)\EnumQueues.exe EnumQueues.cs

$(BINDIR)\TestClient.exe: TestClient.cs
        if not exist $(BINDIR)\. mkdir $(BINDIR)
        $(CSC) $(CSCOPTIONS) /target:exe  /out:$(BINDIR)\TestClient.exe TestClient.cs


$(BINDIR)\TestClient.class: TestClient.java ..\library\$(BINDIR)\MsmqJava.dll
        if not exist $(BINDIR)\. mkdir $(BINDIR)
        $(JAVAC) -g -classpath ..\install\MsmqJava.jar -d $(BINDIR) TestClient.java

clean:
        -rd /s /q $(OBJDIR)
        -rd /s /q $(BINDIR)

