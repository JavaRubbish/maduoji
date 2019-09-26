using System;

public class Class1
{
	public Class1()
	{
	}

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;
            return cp;
        }
    }
}
