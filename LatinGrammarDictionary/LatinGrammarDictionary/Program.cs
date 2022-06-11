using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace LatinGrammarDictionary
{
    public class TxtReader: Form
    {
        TextBox tb;
        CheckBox wrdwrp;
        public TxtReader(string path, string lbl = "Vocabularius grammaticus Latinitatis")
        {
            Text = lbl;
            tb = new TextBox();
            BackColor = Color.BurlyWood;
            tb.Multiline = true;
            tb.WordWrap = true;
            tb.AcceptsTab = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.ReadOnly = true;
            tb.Font = new Font("Courier New", 12);
            wrdwrp = new CheckBox();
            wrdwrp.Text = "Novas lineas facere";
            wrdwrp.Font = new Font("Book Antiqua", 15, FontStyle.Italic);
            wrdwrp.Checked = tb.WordWrap;
            wrdwrp.CheckedChanged += Wrdwrp_CheckedChanged;
            wrdwrp.Height = wrdwrp.Font.Height + 2;
            tb.Location = new Point(10, 10);
            SizeChanged += TxtReader_SizeChanged;
            Height = 450;
            Width = 900;
            wrdwrp.Width = tb.Width;
            tb.Text = "";
            if (path != "")
            {
                StreamReader sr = new StreamReader(path, Pr.utf);
                while (!sr.EndOfStream) tb.Text += sr.ReadLine().Replace("\n","\r\n") + "\r\n";
                //tb.Text = sr.ReadToEnd().Replace("\n","\r\n");
                sr.Close();
                sr.Dispose();
            }
            Controls.Add(tb);
            Controls.Add(wrdwrp);
            MinimumSize = new Size(180 + 2 * tb.Left, tb.Top * 3 + wrdwrp.Height + tb.Font.Height);
        }
        void TxtReader_SizeChanged(object sender, EventArgs e)
        {
            wrdwrp.Location = new Point(20, Height - tb.Top * 6 - wrdwrp.Height);
            tb.Height = wrdwrp.Top - tb.Top * 2;
            tb.Width = Width - tb.Left * 4;
            wrdwrp.Width = tb.Width;
        }
        void Wrdwrp_CheckedChanged(object sender, EventArgs e)
        {
            tb.WordWrap = !tb.WordWrap;
            tb.ScrollBars = tb.WordWrap ? ScrollBars.Vertical : ScrollBars.Both;
        }
    }
    public class FormWithExit : Form
    {
        protected Button exit;
        protected void Exit_Enter(object sender, EventArgs e)
        {
            Close();
        }
    }
    class Form1 : FormWithExit
    {
        static Font f = new Font("Book Antiqua", 15, FontStyle.Italic);
        List<Entry> list;
        ListBox l;
        Label t;
        Button b;
        Button quick;
        public Form1(SortedSet<Entry> dict)
        {
            BackColor = Color.BurlyWood;
            Text = "Vocabularii editor";
            MaximizeBox = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Height = 450;
            t = new Label();
            t.Text = "Vocabulae:";
            t.Font = f;
            t.Location = new Point(10, 10);
            t.Height = 35;
            t.Width = t.Text.Length * 12 + 20;
            l = new ListBox();
            list = new List<Entry>();
            foreach(Entry e in dict)
            {
                list.Add(e);
            }
            l.DataSource = list;
            l.Location = new Point(t.Left, 50);
            l.SelectionMode = SelectionMode.One;
            l.Width = 800;
            l.Height = 250;
            l.UseTabStops = true;
            b = new Button();
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Font = f;
            b.Text = "Adde irregularitatem";
            b.Height = t.Height;
            b.Width = b.Text.Length * 12 + 10;
            b.Location = new Point(l.Left + 50, l.Bottom + 20);
            b.Enter += B_Enter;
            quick = new Button();
            quick.TextAlign = ContentAlignment.MiddleCenter;
            quick.Font = f;
            quick.Text = "Mutatio minor";
            quick.Height = b.Height;
            quick.Width = quick.Text.Length * 12 + 20;
            quick.Location = new Point((l.Left + l.Width - 
                quick.Width) / 2, b.Top);
            quick.Enter += Quick_Enter;
            exit = new Button();
            exit.TextAlign = ContentAlignment.MiddleCenter;
            exit.Font = f;
            exit.Text = "Finire volo";
            exit.Height = b.Height;
            exit.Width = exit.Text.Length * 12 + 10;
            exit.Location = new Point(l.Right - 50 - exit.Width, b.Top);
            exit.Enter += Exit_Enter;
            Controls.Add(l);
            Controls.Add(t);
            Controls.Add(b);
            Controls.Add(quick);
            Controls.Add(exit);
            Width = l.Width + 50;
            if (dict.Count == 0)
            {
                quick.Enabled = false;
                b.Enabled = false;
            }
            AcceptButton = b;
        }
        static void IndeclMessageBoxShow(string baseform)
        {
            MessageBox.Show("Indeclinabile. Nihil mutari potest.",
                baseform, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
        }
        void Quick_Enter(object sender, EventArgs ea)
        {
            Entry e = (Entry)l.SelectedItem;
            StreamWriter sw = new StreamWriter(Pr.irregfile, true, Pr.utf);
            sw.WriteLine();
            switch (e.POS)
            {
                case Entry.PoS.Indecl: IndeclMessageBoxShow(e.BaseForm);
                    break;
                case Entry.PoS.Verb:
                    switch (MessageBox.Show("Habetne supinos?",
                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            e.NoSupins = false;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|yessups|");
                                break;
                        case DialogResult.No:
                            e.NoSupins = true;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|nosups|");
                            break;
                    }
                    switch (MessageBox.Show("Habetne utramque vocem?",
                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            e.Intrans = false;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|tran|");
                            break;
                        case DialogResult.No:
                            e.Intrans = true;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|intr|");
                            break;
                    }
                    break;
                case Entry.PoS.Noun:
                    if (!e.isPlT)
                        switch (MessageBox.Show("Estne singulare tantum?",
                            e.BaseForm, MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: e.NoPlural = true;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|nopl|");
                                break;
                            case DialogResult.No:
                                e.NoPlural = false;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|yespl|");
                                break;
                        }
                    if (!e.Pronominal)
                        switch (MessageBox.Show("Declinaturne Graece?",
                            e.BaseForm, MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: e.GreekDecl = true;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|gr|");
                                break;
                            case DialogResult.No: e.GreekDecl = false;
                                sw.WriteLine(e.ToString().Substring(0,
                                    e.ToString().IndexOf('\t')) + "|nogr|");
                                break;
                        }
                    if (!e.GreekDecl & !e.Pronominal)
                        switch (MessageBox.Show("Declinaturne sicut iuvenis?",
                            e.BaseForm, MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes:
                                e.isIuvenisType = true;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|3cons|");
                                break;
                            case DialogResult.No:
                                e.isIuvenisType = false;
                                if (Pr.EW(e.BaseForm, "is"))
                                    switch (MessageBox.Show("Declinaturne sicut turris?",
                                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                                    {
                                        case DialogResult.Yes:
                                            e.isTurrisType = true;
                                            sw.WriteLine(e.ToString().Substring(0, 
                                                e.ToString().IndexOf('\t')) + "|3voc|");
                                            break;
                                        case DialogResult.No:
                                            e.isTurrisType = false;
                                            sw.WriteLine(e.ToString().Substring(0, 
                                                e.ToString().IndexOf('\t')) + "|3mixta|");
                                            break;
                                    }
                                break;
                            case DialogResult.Cancel:
                                if (Pr.EW(e.BaseForm, "is"))
                                    switch (MessageBox.Show("Declinaturne sicut turris?",
                                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                                    {
                                        case DialogResult.Yes:
                                            e.isTurrisType = true;
                                            sw.WriteLine(e.ToString().Substring(0, 
                                                e.ToString().IndexOf('\t')) + "|3voc|");
                                            break;
                                        case DialogResult.No:
                                            e.isTurrisType = false;
                                            sw.WriteLine(e.ToString().Substring(0, 
                                                e.ToString().IndexOf('\t')) + "|3mixta|");
                                            break;
                                    }
                                break;
                        }
                    switch (MessageBox.Show("Habetne locativum in -ī?",
                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            e.iLocative = true;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|iloc|");
                            break;
                        case DialogResult.No:
                            e.iLocative = false;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|normloc|");
                            break;
                    }
                    break;
                default:
                    switch (MessageBox.Show("Declinaturne sicut unus?",
                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            e.Pronominal = true;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|pron|");
                            break;
                        case DialogResult.No:
                            e.Pronominal = false;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|npron|");
                            break;
                    }
                    if (!e.Pronominal & (e.POS == Entry.PoS.Adj))
                        switch (MessageBox.Show("Declinaturne Graece?",
                            e.BaseForm, MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes:
                                e.GreekDecl = true;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|gr|");
                                break;
                            case DialogResult.No:
                                e.GreekDecl = false;
                                sw.WriteLine(e.ToString().Substring(0, 
                                    e.ToString().IndexOf('\t')) + "|nogr|");
                                break;
                        }
                    if (!e.GreekDecl & !e.Pronominal)
                        switch (MessageBox.Show("Estne participium?",
                        e.BaseForm, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            e.isTurrisType = true;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|3voc|");
                            break;
                        case DialogResult.No:
                            e.isTurrisType = false;
                            sw.WriteLine(e.ToString().Substring(0, 
                                e.ToString().IndexOf('\t')) + "|3mixta|");
                            break;
                    }
                    break;
            }
            sw.Close();
            sw.Dispose();
        }
        class Form2 : FormWithExit
        {
            Entry e;
            Button b;
            Button advsup;
            Button pfa;
            TextBox t;
            Label ctlabel;
            ListBox casetense;
            Label gmlabel;
            ListBox gendermood;
            CheckBox plural;
            CheckBox variant;
            CheckBox passive;
            CheckBox first;
            CheckBox second;
            public Form2(Entry e)
            {
                BackColor = Color.BurlyWood;
                this.e = e;
                Text = e.BaseForm;
                MaximizeBox = false;
                SizeGripStyle = SizeGripStyle.Hide;
                t = new TextBox();
                t.Location = new Point(10, 10);
                t.Font = f;
                t.Height = 35;
                ctlabel = new Label();
                ctlabel.Font = f;
                ctlabel.Height = t.Height;
                ctlabel.Width = 100;
                ctlabel.Location = new Point(t.Left, t.Bottom + 10);
                Controls.Add(ctlabel);
                casetense = new ListBox();
                casetense.Width = 150;
                casetense.Height = 150;
                casetense.Location = new Point(ctlabel.Left,
                    ctlabel.Bottom);
                casetense.SelectionMode = SelectionMode.One;
                Controls.Add(casetense);
                t.Width = casetense.Width * 4;
                Controls.Add(t);
                gmlabel = new Label();
                gmlabel.Font = f;
                gmlabel.Height = t.Height;
                gmlabel.Width = 100;
                gmlabel.Location = new Point(casetense.Right + 20,
                    ctlabel.Top);
                Controls.Add(gmlabel);
                gendermood = new ListBox();
                gendermood.Width = 150;
                gendermood.Height = 100;
                gendermood.Location = new Point(gmlabel.Left,
                    casetense.Top);
                gendermood.SelectionMode = SelectionMode.One;
                Controls.Add(gendermood); //Must be before the lock for nouns
                variant = new CheckBox();
                variant.Font = f;
                variant.Text = "vel sicut antea";
                variant.TextAlign = ContentAlignment.MiddleLeft;
                variant.Height = t.Height;
                variant.Width = variant.Text.Length * 12 + 100;
                variant.Location = new Point(gendermood.Right + 20,
                    casetense.Top);
                Controls.Add(variant);
                plural = new CheckBox();
                plural.Font = f;
                plural.Text = "Pluralis (si habet)";
                plural.TextAlign = ContentAlignment.MiddleLeft;
                plural.Height = variant.Height;
                plural.Width = plural.Text.Length * 12 + 100;
                plural.Location = new Point(variant.Left, variant.Bottom + 20);
                Controls.Add(plural);
                b = new Button();
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = f;
                b.Text = "Adde";
                b.Width = b.Text.Length * 12 + 40;
                b.Height = t.Height;
                b.Location = new Point(casetense.Left, casetense.Bottom + 10);
                b.Enter += Irreg_Enter;
                Controls.Add(b);
                exit = new Button();
                exit.TextAlign = ContentAlignment.MiddleCenter;
                exit.Font = f;
                exit.Text = "Finire volo";
                exit.Height = b.Height;
                exit.Width = exit.Text.Length * 12 + 10;
                exit.Location = new Point(t.Right - 50 - exit.Width, b.Top);
                exit.Enter += Exit_Enter;
                Controls.Add(exit);
                advsup = new Button();
                advsup.TextAlign = ContentAlignment.MiddleCenter;
                advsup.Font = f;
                advsup.Height = b.Height;
                advsup.Location = new Point(b.Right + 10, b.Top);
                if (e.Nomen)
                {
                    casetense.DataSource = LatinNoun.CaseNames;
                    ctlabel.Text = "Casus";
                    gendermood.DataSource = LatinNoun.GenderNames;
                    gmlabel.Text = "Genus";
                    if (e.POS != Entry.PoS.Noun)
                    {
                        advsup.Text = "Adverbium";
                        advsup.Enter += Adv_Enter;
                    }
                    else
                    {
                        advsup.Text = "Nomen est";
                        advsup.Enabled = advsup.Visible = false;
                        string g = e.Gender;
                        foreach (string s in LatinNoun.GenderNames)
                        {
                            if (g.Contains(s))
                                //gendermood.SelectedItem = s;
                                gendermood.SelectedItems.Add(s);
                        }
                        gendermood.Enabled = false;
                    }
                    advsup.Width = advsup.Text.Length * 12 + 40; //see in else
                }
                else
                {
                    casetense.DataSource = LatinVerb.TenseNames;
                    ctlabel.Text = "Tempus";
                    gendermood.DataSource = LatinVerb.MoodNames;
                    gmlabel.Text = "Modus";
                    advsup.Text = "Supinum";
                    advsup.Enter += Sup_Enter;
                    advsup.Width = advsup.Text.Length * 12 + 40; //must be before pfa.Location
                    if (!e.NoPassive)
                    {
                        passive = new CheckBox();
                        passive.Font = f;
                        passive.Text = "Vox passiva";
                        passive.TextAlign = ContentAlignment.MiddleLeft;
                        passive.Height = variant.Height;
                        passive.Location = new Point(variant.Left,
                            plural.Bottom + 20);
                        passive.Width = passive.Text.Length * 12 + 100;
                        Controls.Add(passive);
                    }
                    if (!e.NoPlural) //Checks for verba defectiva
                    {
                        first = new CheckBox();
                        first.Font = f;
                        first.Text = "1 p.";
                        first.TextAlign = ContentAlignment.MiddleLeft;
                        first.Height = variant.Height;
                        first.Location = new Point(gendermood.Left,
                            gendermood.Bottom + 10);
                        first.Width = gendermood.Width / 2 - 5;
                        Controls.Add(first);
                        second = new CheckBox();
                        second.Font = f;
                        second.Text = "2 p.";
                        second.TextAlign = ContentAlignment.MiddleLeft;
                        second.Height = variant.Height;
                        second.Location = new Point(first.Right + 5,
                            gendermood.Bottom + 10);
                        second.Width = first.Width;
                        Controls.Add(second);
                    }
                    pfa = new Button();
                    pfa.TextAlign = ContentAlignment.MiddleCenter;
                    pfa.Font = f;
                    pfa.Height = b.Height;
                    pfa.Location = new Point(advsup.Right + 10, b.Top);
                    pfa.Text = "PFA";
                    pfa.Enter += Pfa_Enter;
                    pfa.Width = pfa.Text.Length * 12 + 40;
                    Controls.Add(pfa);
                }
                if (e.isPlT | e.NoPlural)
                {
                    plural.Enabled = false;
                    plural.Checked = e.isPlT;
                }
                if (e.POS != Entry.PoS.Noun) Controls.Add(advsup);
                AcceptButton = b;
                Width = t.Width + 50;
                Height = b.Bottom + 70;
            }
            void Irreg_Enter(object sender, EventArgs ea)
            {
                StreamWriter sw = new StreamWriter(Pr.irregfile, true, Pr.utf);
                sw.WriteLine();
                if (e.POS == Entry.PoS.Verb)
                {
                    LatinVerb.Tense tns = LatinVerb.Tense.Prs;
                    for (int i = 1; i < LatinVerb.TenseNames.Count; i++)
                    {
                        if (LatinVerb.TenseNames[i] ==
                            casetense.SelectedItem.ToString())
                        {
                            tns = (LatinVerb.Tense)i;
                            break;
                        }
                    }
                    LatinVerb.Mood m = LatinVerb.Mood.Inf;
                    for (int i = 1; i < LatinVerb.MoodNames.Count; i++)
                    {
                        if (LatinVerb.MoodNames[i] ==
                            gendermood.SelectedItem.ToString())
                        {
                            m = (LatinVerb.Mood)i;
                            break;
                        }
                    }
                    byte p = 0;
                    if (second.Checked) p = 2;
                    if (first.Checked) p = 1;
                    string s = "";
                    switch (tns)
                    {
                        case LatinVerb.Tense.Prs: s += "prs "; break;
                        case LatinVerb.Tense.Impf: s += "impf "; break;
                        case LatinVerb.Tense.FutI: s += "fut "; break;
                        case LatinVerb.Tense.Pfct: s += "pfct "; break;
                        case LatinVerb.Tense.Plpf: s += "plpf "; break;
                        case LatinVerb.Tense.FutII: s += "futII "; break;
                    }
                    s += gendermood.SelectedItem
                        .ToString().ToLower().Substring(0, 3);
                    if (m != LatinVerb.Mood.Inf)
                    {
                        if (plural.Checked) s += " pl";
                        if (p != 0) s += " " + p.ToString();
                    }
                    if (passive.Checked) s += " pass";
                    if (t.Text == "")
                    {
                        e.SetVerbalLacuna(tns, m, p, plural.Checked,
                            passive.Checked);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|");
                    }
                    else if (variant.Checked)
                    {
                        e.AddVerbalVariantForm(tns, m, p, plural.Checked,
                            passive.Checked, t.Text);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|" 
                            + t.Text + "+");
                    }
                    else {
                        e.AddVerbalIrregularForm(tns, m, p, plural.Checked,
                            passive.Checked, t.Text);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|"
                            + t.Text);
                    }
                }
                else
                {
                    LatinNoun.Case c = LatinNoun.Case.Voc;
                    for (int i = 1; i < LatinNoun.CaseNames.Count; i++)
                    {
                        if (LatinNoun.CaseNames[i] ==
                            casetense.SelectedItem.ToString())
                        {
                            c = (LatinNoun.Case)i;
                            break;
                        }
                    }
                    LatinNoun.Gend g = LatinNoun.Gend.N;
                    for (int i = 1; i < LatinNoun.GenderNames.Count; i++)
                    {
                        if (LatinNoun.GenderNames[i] ==
                            gendermood.SelectedItem.ToString())
                        {
                            if (plural.Checked) g = (LatinNoun.Gend)(i + 4);
                            else g = (LatinNoun.Gend)i;
                            break;
                        }
                    }
                    string s = "";
                    if (plural.Checked) s += "pl ";
                    s += gendermood.SelectedItem.ToString()[0] + "," +
                        casetense.SelectedItem.ToString().ToLower()
                        .Substring(0, 3);
                    if (t.Text == "")
                    {
                        e.SetNominalLacuna(c, g);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|");
                    }
                    else if (variant.Checked)
                    {
                        e.AddNominalVariantForm(c, g, t.Text);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|"
                            + t.Text + "+");
                    }
                    else {
                        e.AddNominalIrregularForm(c, g, t.Text);
                        sw.WriteLine(e.ToString().Substring(0, 
                            e.ToString().IndexOf('\t')) + "|" + s + "|"
                            + t.Text);
                    }
                }
                sw.Close();
                sw.Dispose();
            }
            void Pfa_Enter(object sender, EventArgs ea)
            {
                if (Pr.EW(t.Text, "urus"))
                    t.Text = t.Text.Substring(0, t.Text.Length - 4);
                else if (Pr.EW(t.Text, "ur"))
                    t.Text = t.Text.Substring(0, t.Text.Length - 2);
                if (variant.Checked)
                {
                    e.PFAStem = e.PFAStem + "-/" + t.Text;
                    if (t.Text != "") t.Text = t.Text + "+";
                }
                else e.PFAStem = t.Text;
                StreamWriter sw = new StreamWriter(Pr.irregfile, true, Pr.utf);
                sw.WriteLine();
                sw.WriteLine(e.ToString().Substring(0, 
                    e.ToString().IndexOf('\t')) + "|pfa|" + t.Text);
                sw.Close();
                sw.Dispose();
                if (variant.Checked & (t.Text.Length > 0))
                    t.Text = t.Text.Substring(0, t.Text.Length - 1);
            }
            void Adv_Enter(object sender, EventArgs ea)
            {
                if (variant.Checked)
                {
                    e.Adverbium = e.Adverbium + "/" + t.Text;
                    if (t.Text != "") t.Text = t.Text + "+";
                }
                else e.Adverbium = t.Text;
                StreamWriter sw = new StreamWriter(Pr.irregfile, true, Pr.utf);
                sw.WriteLine();
                sw.WriteLine(e.ToString().Substring(0, 
                    e.ToString().IndexOf('\t')) + "|adv|" + t.Text);
                sw.Close();
                sw.Dispose();
                if (variant.Checked & (t.Text.Length > 0))
                    t.Text = t.Text.Substring(0, t.Text.Length - 1);
            }
            void Sup_Enter(object sender, EventArgs ea)
            {
                if (Pr.EW(t.Text, "um"))
                    t.Text = t.Text.Substring(0, t.Text.Length - 2);
                else if (Pr.EW(t.Text, "u"))
                    t.Text = t.Text.Substring(0, t.Text.Length - 1);
                if (variant.Checked)
                {
                    e.SupinStem = e.SupinStem + "-/" + t.Text;
                    if (t.Text != "") t.Text = t.Text + "+";
                }
                else e.SupinStem = t.Text;
                e.NoSupins = false;
                StreamWriter sw = new StreamWriter(Pr.irregfile, true, Pr.utf);
                sw.WriteLine();
                sw.WriteLine(e.ToString().Substring(0, 
                    e.ToString().IndexOf('\t')) + "|sup|" + t.Text);
                sw.Close();
                sw.Dispose();
                if (variant.Checked & (t.Text.Length > 0))
                    t.Text = t.Text.Substring(0, t.Text.Length - 1);
            }
        }
        void B_Enter(object sender, EventArgs ea)
        {
            Entry e = (Entry)l.SelectedItem;
            if (e.isInflectable) (new Form2(e)).ShowDialog();
            else IndeclMessageBoxShow(e.BaseForm);
            l.DataSource = list;
        }
    }
    class Entry
    {
        public bool EndsWith(string s)
        {
            if (type == null) return false;
            switch (pos)
            {
                case PoS.Indecl: return Pr.EW(type.ToString(), s);
                case PoS.Verb:
                    LatinVerb v = (LatinVerb)type;
                    return v.EndsWith(s);
                case PoS.Noun:
                    LatinNoun x = (LatinNoun)type;
                    return x.EndsWith(s);
                default:
                    LatinAdj y = (LatinAdj)type;
                    return y.EndsWith(s);
            }
        }
        public string Gender
        {
            get
            {
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    return x.Gender;
                }
                else return "Genus non habet";
            }
        }
        public string SupinStem
        {
            get
            {
                if (pos != PoS.Verb) return "";
                LatinVerb v = (LatinVerb)type;
                return v.SupinStem;
            }
            set
            {
                if (pos == PoS.Verb)
                {
                    LatinVerb v = (LatinVerb)type;
                    v.SupinStem = value;
                    type = v;
                }
            }
        }
        public string PFAStem
        {
            get
            {
                if (pos != PoS.Verb) return "";
                LatinVerb v = (LatinVerb)type;
                return v.PFAStem;
            }
            set
            {
                if (pos == PoS.Verb)
                {
                    LatinVerb v = (LatinVerb)type;
                    v.PFAStem = value;
                    type = v;
                }
            }
        }
        public string Adverbium
        {
            get
            {
                switch (pos)
                {
                    case PoS.Indecl: return ToString();
                    case PoS.Verb:
                        LatinVerb v = (LatinVerb)type;
                        return v.PPA.Adverbium;
                    case PoS.Noun:
                        LatinNoun x = (LatinNoun)type;
                        return x.OneForm(LatinNoun.Case.Abl, false);
                    default:
                        LatinAdj y = (LatinAdj)type;
                        return y.Adverbium;
                }
            }
            set
            {
                if ((pos == PoS.Adj) | (pos == PoS.Num))
                {
                    LatinAdj y = (LatinAdj)type;
                    y.Adverbium = value;
                    type = y;
                }
            }
        }
        public bool Intrans
        {
            get
            {
                if (pos != PoS.Verb) return false;
                LatinVerb v = (LatinVerb)type;
                return v.Intrans;
            }
            set
            {
                if (pos == PoS.Verb)
                {
                    LatinVerb v = (LatinVerb)type;
                    v.Intrans = value;
                    type = v;
                }
            }
        }
        public bool iLocative
        {
            get
            {
                if (pos != PoS.Noun) return false;
                LatinNoun x = (LatinNoun)type;
                return x.iLocative;
            }
            set
            {
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    x.iLocative = value;
                    type = x;
                }
            }
        }
        object type;
        /*public void requestFull() //Doesn't scroll => useless.
        {
            MessageBox.Show(Inflect(), BaseForm, MessageBoxButtons.OK);
        }*/
        PoS pos;
        public bool NoSupins
        {
            get
            {
                if (pos != PoS.Verb) return false;
                LatinVerb v = (LatinVerb)type;
                return v.NoSupins;
            }
            set
            {
                if (pos == PoS.Verb)
                {
                    LatinVerb v = (LatinVerb)type;
                    v.NoSupins = value;
                    type = v;
                }
            }
        }
        public bool NoPlural
        {
            get
            {
                if (pos == PoS.Verb)
                {
                    LatinVerb v = (LatinVerb)type;
                    if (v.OneForm(LatinVerb.Tense.Pfct, LatinVerb.Mood.Ind,
                           1, true, false) == "")
                        return v.OneForm(LatinVerb.Tense.Prs,
                            LatinVerb.Mood.Ind, 1, true, false) ==
                            v.OneForm(LatinVerb.Tense.Prs, LatinVerb.Mood.Ind,
                            0, false, false);
                    return v.OneForm(LatinVerb.Tense.Pfct,
                        LatinVerb.Mood.Ind, 1, true, false) ==
                        v.OneForm(LatinVerb.Tense.Pfct, LatinVerb.Mood.Ind,
                        0, false, false);
                }
                if (pos != PoS.Noun) return false;
                LatinNoun x = (LatinNoun)type;
                return x.NoPlural;
            }
            set
            {
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    x.NoPlural = value;
                    type = x;
                }
            }
        }
        public bool NoPassive
        {
            get
            {
                if (pos != PoS.Verb) return false;
                LatinVerb v = (LatinVerb)type;
                if (v.Deponens) return true;
                if (v.Semideponens) return true;
                return v.EndsWith("sum");
            }
        }
        public void AddNominalIrregularForm(LatinNoun.Case c,
            LatinNoun.Gend g, string s)
        {
            if (pos == PoS.Noun)
            {
                LatinNoun x = (LatinNoun)type;
                x.AddIrregularForm(c, LatinNoun.IsPluralGender(g), s);
                type = x;
            }
            else if ((pos == PoS.Adj) | (pos == PoS.Num))
            {
                LatinAdj y = (LatinAdj)type;
                y.AddIrregularForm(c, g, s);
                type = y;
            }
        }
        public void AddVerbalIrregularForm(LatinVerb.Tense t,
            LatinVerb.Mood m, byte p, bool plural, bool passive, string s)
        {
            if (pos == PoS.Verb)
            {
                LatinVerb v = (LatinVerb)type;
                v.AddIrregularForm(t, m, p, plural, passive, s);
                type = v;
            }
        }
        public void AddNominalVariantForm(LatinNoun.Case c,
            LatinNoun.Gend g, string s)
        {
            if (pos == PoS.Noun)
            {
                LatinNoun x = (LatinNoun)type;
                x.AddVariantForm(c, LatinNoun.IsPluralGender(g), s);
                type = x;
            }
            else if ((pos == PoS.Adj) | (pos == PoS.Num))
            {
                LatinAdj y = (LatinAdj)type;
                y.AddVariantForm(c, g, s);
                type = y;
            }
        }
        public void AddVerbalVariantForm(LatinVerb.Tense t,
            LatinVerb.Mood m, byte p, bool plural, bool passive, string s)
        {
            if (pos == PoS.Verb)
            {
                LatinVerb v = (LatinVerb)type;
                v.AddVariantForm(t, m, p, plural, passive, s);
                type = v;
            }
        }
        public void SetNominalLacuna(LatinNoun.Case c,
            LatinNoun.Gend g)
        {
            if (pos == PoS.Noun)
            {
                LatinNoun x = (LatinNoun)type;
                x.SetLacuna(c, LatinNoun.IsPluralGender(g));
                type = x;
            }
            else if ((pos == PoS.Adj) | (pos == PoS.Num))
            {
                LatinAdj y = (LatinAdj)type;
                y.SetLacuna(c, g);
                type = y;
            }
        }
        public void SetVerbalLacuna(LatinVerb.Tense t,
            LatinVerb.Mood m, byte p, bool plural, bool passive)
        {
            if (pos == PoS.Verb)
            {
                LatinVerb v = (LatinVerb)type;
                v.SetLacuna(t, m, p, plural, passive);
                type = v;
            }
        }
        public PoS POS
        {
            get
            {
                if ((type == null) | (type.GetType() == "".GetType()))
                {
                    pos = PoS.Indecl;
                }
                return pos;
            }
        }
        public bool isPlT
        {
            get
            {
                if (pos == PoS.Num) return true;
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    return x.IsPlT;
                }
                return false;
            }
        }
        public bool Pronominal
        {
            get
            {
                switch (pos)
                {
                    case PoS.Noun:
                        LatinNoun x = (LatinNoun)type;
                        return x.Pronominal;
                    case PoS.Adj:
                        LatinAdj y = (LatinAdj)type;
                        return y.Pronominal;
                    default: return false;
                }
            }
            set
            {
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    x.Pronominal = value;
                    type = x;
                }
                else if (pos == PoS.Adj)
                {
                    LatinAdj y = (LatinAdj)type;
                    y.Pronominal = value;
                    type = y;
                }
            }
        }
        public bool isInflectable
        {
            get { return POS != PoS.Indecl; }
        }
        public bool Nomen
        {
            get { return isInflectable & (POS != PoS.Verb); }
        }
        public bool isTurrisType
        {
            get
            {
                if (!Nomen) return false;
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    return x.isTurrisType;
                }
                LatinAdj y = (LatinAdj)type;
                return (y.Declension == "3 vocalis");
            }
            set
            {
                if (Nomen)
                    if (pos == PoS.Noun)
                    {
                        LatinNoun x = (LatinNoun)type;
                        x.isTurrisType = value;
                        type = x;
                    }
                    else
                    {
                        LatinAdj y = (LatinAdj)type;
                        y.Participium = !value;
                        type = y;
                    }
            }
        }
        public bool isIuvenisType
        {
            get
            {
                if (!Nomen) return false;
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    return x.isIuvenisType;
                }
                LatinAdj y = (LatinAdj)type;
                return y.isIuvenisType;
            }
            set
            {
                if (Nomen)
                    if (pos == PoS.Noun)
                    {
                        LatinNoun x = (LatinNoun)type;
                        x.isIuvenisType = value;
                        type = x;
                    }
                    else
                    {
                        LatinAdj y = (LatinAdj)type;
                        y.isIuvenisType = value;
                        type = y;
                    }
            }
        }
        public string BaseForm
        {
            get
            {
                if (type == null) return "";
                switch (pos)
                {
                    case PoS.Indecl: return type.ToString();
                    case PoS.Noun:
                        LatinNoun x = (LatinNoun)type;
                        return x.Nominative;
                    case PoS.Verb:
                        LatinVerb y = (LatinVerb)type;
                        return y.BaseForm;
                    default:
                        LatinAdj z = (LatinAdj)type;
                        return z.Nominative;
                }
            }
        }
        public bool GreekDecl
        {
            get
            {
                switch (pos)
                {
                    case PoS.Indecl: return false;
                    case PoS.Verb: return false;
                    case PoS.Noun:
                        LatinNoun x = (LatinNoun)type;
                        return x.GreekDecl;
                    default:
                        LatinAdj y = (LatinAdj)type;
                        return y.GreekDecl;
                }
            }
            set
            {
                if (pos == PoS.Noun)
                {
                    LatinNoun x = (LatinNoun)type;
                    x.GreekDecl = value;
                    type = x;
                }
                else if (pos == PoS.Adj) //Numerals never have Greek decl.
                {
                    LatinAdj y = (LatinAdj)type;
                    y.GreekDecl = value;
                    type = y;
                }
            }
        }
        public override string ToString()
        {
            if (type == null) return "";
            switch (pos)
            {
                case PoS.Indecl: return type.ToString();
                case PoS.Noun:
                    LatinNoun x = (LatinNoun)type;
                    return x.ToString().Replace('\n', '\t');
                case PoS.Verb:
                    LatinVerb y = (LatinVerb)type;
                    return y.ToString().Replace('\n', '\t');
                default:
                    LatinAdj z = (LatinAdj)type;
                    return z.ToString().Replace('\n', '\t');
            }
        }
        public enum PoS { Indecl = 0, Noun = 1, Adj = 2, Verb = 3, Num = 4 }
        public Entry(PoS entrypos, string[] splitresults)
        {
            pos = entrypos;
            if (splitresults.Length > 0)
            {
                string stem;
                switch (entrypos)
                {
                    case PoS.Verb:
                        switch (splitresults.Length)
                        {
                            case 1:
                                type = new LatinVerb(splitresults[0]);
                                break;
                            case 2:
                                if (Pr.Delen(splitresults[1]) == "isse")
                                    type = new LatinVerb(splitresults[0]);
                                type = new LatinVerb(splitresults[0],
                                    splitresults[1]); break;
                            case 3:
                                stem = splitresults[0].Substring(0,
                                    splitresults[0].Length - 1);
                                if (splitresults[0].EndsWith("r") |
                                    splitresults[0].EndsWith("t"))
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.EW(splitresults[0], "tur"))
                                    stem = splitresults[0].Substring(0,
                                    splitresults[0].Length - 3);
                                if (Pr.Delen(splitresults[2]) == "are")
                                    splitresults[2] = stem + "āre";
                                if (Pr.Delen(splitresults[2]) == "ari")
                                    splitresults[2] = stem + "ārī";
                                if (Pr.Delen(splitresults[1]) == "avi")
                                    splitresults[1] = stem + "āvī";
                                if (Pr.Delen(splitresults[1]) == "avit")
                                    splitresults[1] = stem + "āvit";
                                if (Pr.Delen(splitresults[1]) == "atus sum")
                                    splitresults[1] = stem + "ātus sum";
                                if (Pr.Delen(splitresults[1]) == "atum est")
                                    splitresults[1] = stem + "ātum est";
                                if (Pr.EW(stem, "i")) //Hic, cf. cruciare
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.EW(stem, "e")) //Hic, cf. commeare
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.Delen(splitresults[2]) == "ire")
                                    splitresults[2] = stem + "īre";
                                if (Pr.Delen(splitresults[2]) == "iri")
                                    splitresults[2] = stem + "īrī";
                                if (Pr.Delen(splitresults[1]) == "ivi")
                                    splitresults[1] = stem + "īvī";
                                if (Pr.Delen(splitresults[1]) == "ivit")
                                    splitresults[1] = stem + "īvit";
                                if (Pr.Delen(splitresults[1]) == "itus sum")
                                    splitresults[1] = stem + splitresults[1];
                                if (Pr.Delen(splitresults[1]) == "itum est")
                                    splitresults[1] = stem + splitresults[1];
                                if (Pr.Delen(splitresults[2]) == "ere")
                                    splitresults[2] = stem + "ere"; //2 et 3
                                if (Pr.Delen(splitresults[2]) == "eri")
                                {
                                    stem = stem.Substring(0,
                                            stem.Length - 1);
                                    splitresults[2] = stem + "erī";
                                }
                                if (Pr.Delen(splitresults[1]) == "evi")
                                    splitresults[1] = stem + "ēvī";
                                if (Pr.Delen(splitresults[1]) == "evit")
                                    splitresults[1] = stem + "ēvit";
                                if (Pr.Delen(splitresults[1]) == "ui")
                                    splitresults[1] = stem + "uī";
                                if (Pr.Delen(splitresults[1]) == "uit")
                                    splitresults[1] = stem + "uit";
                                if (Pr.Delen(splitresults[1]) == "etus sum")
                                    splitresults[1] = stem + "ētus sum";
                                if (Pr.Delen(splitresults[1]) == "etum est")
                                    splitresults[1] = stem + "ētum est";
                                if (Pr.Delen(splitresults[2]) == "i")
                                    splitresults[2] = stem + "ī";
                                type = new LatinVerb(splitresults[0],
                                    splitresults[1], splitresults[2]);
                                break;
                            default:
                                stem = splitresults[0].Substring(0,
                                    splitresults[0].Length - 1);
                                if (splitresults[0].EndsWith("r") |
                                    splitresults[0].EndsWith("t"))
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.EW(splitresults[0], "tur"))
                                    stem = splitresults[0].Substring(0,
                                    splitresults[0].Length - 3);
                                if (Pr.Delen(splitresults[3]) == "are")
                                    splitresults[3] = stem + "āre";
                                if (Pr.Delen(splitresults[3]) == "ari")
                                    splitresults[3] = stem + "ārī";
                                if (Pr.Delen(splitresults[1]) == "avi")
                                    splitresults[1] = stem + "āvī";
                                if (Pr.Delen(splitresults[1]) == "avit")
                                    splitresults[1] = stem + "āvit";
                                if (Pr.Delen(splitresults[2]) == "atum")
                                    splitresults[2] = stem + "ātum";
                                if (Pr.EW(stem, "i")) //Hic, cf. cruciare
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.EW(stem, "e")) //Hic, cf. commeare
                                    stem = stem.Substring(0,
                                        stem.Length - 1);
                                if (Pr.Delen(splitresults[3]) == "ire")
                                    splitresults[3] = stem + "īre";
                                if (Pr.Delen(splitresults[3]) == "iri")
                                    splitresults[3] = stem + "īrī";
                                if (Pr.Delen(splitresults[1]) == "ivi")
                                    splitresults[1] = stem + "īvī";
                                if (Pr.Delen(splitresults[1]) == "ivit")
                                    splitresults[1] = stem + "īvit";
                                if (Pr.Delen(splitresults[2]) == "itum")
                                    splitresults[2] = stem + splitresults[1];
                                if (Pr.Delen(splitresults[3]) == "ere")
                                    splitresults[3] = stem + "ere"; //2 et 3
                                if (Pr.Delen(splitresults[3]) == "eri")
                                {
                                    stem = stem.Substring(0,
                                            stem.Length - 1);
                                    splitresults[3] = stem + "erī";
                                }
                                if (Pr.Delen(splitresults[1]) == "evi")
                                    splitresults[1] = stem + "ēvī";
                                if (Pr.Delen(splitresults[1]) == "evit")
                                    splitresults[1] = stem + "ēvit";
                                if (Pr.Delen(splitresults[1]) == "ui")
                                    splitresults[1] = stem + "uī";
                                if (Pr.Delen(splitresults[1]) == "uit")
                                    splitresults[1] = stem + "uit";
                                if (Pr.Delen(splitresults[2]) == "etum")
                                    splitresults[2] = stem + "ētum";
                                if (Pr.Delen(splitresults[3]) == "i")
                                    splitresults[3] = stem + "ī";
                                type = new LatinVerb(splitresults[0],
                                    splitresults[1], splitresults[2],
                                    splitresults[3]); break;
                        }
                        break;
                    case PoS.Indecl:
                        type = splitresults[0]; break;
                    case PoS.Noun:
                        string nom = splitresults[0];
                        string gen;
                        if (splitresults.Length == 1) gen = splitresults[0];
                        else gen = splitresults[1];
                        if (gen != nom)
                        {
                            stem = nom.Substring(0, nom.Length - 2);
                            if (Pr.IsLatinVowel(nom[nom.Length - 1]) &
                                !Pr.EW(nom, "ae"))
                                stem = stem + nom[nom.Length - 2];
                            else if (Pr.Delen(gen) == "is")
                            {
                                if (!Pr.EW(nom, "s"))
                                    stem = nom;
                                gen = stem + gen;
                            }
                            if ((Pr.Delen(gen) == "ae") | (Pr.Delen(gen) == "ei")
                                | (Pr.Delen(gen) == "us") | (Pr.Delen(gen) ==
                                "arum") | (Pr.Delen(gen) == "um") | (Pr.Delen(gen)
                                == "orum") | (Pr.Delen(gen) == "uum") |
                                (Pr.Delen(gen) == "erum")) gen = stem + gen;
                            if (Pr.Delen(gen) == "i")
                            {
                                if (!Pr.EW(nom, "s") & !Pr.EW(nom, "m"))
                                    stem = nom;
                                gen = stem + gen;
                            }
                        }
                        type = new LatinNoun(nom, gen); break;
                    default:
                        if (splitresults.Length == 1)
                            splitresults = new string[2] { splitresults[0],
                                splitresults[0] };
                        if (splitresults.Length == 2)
                        {
                            if (splitresults[1] == "e")
                                splitresults[1] = splitresults[0]
                                    .Substring(0, splitresults[0].Length
                                    - 2) + "ĕ";
                            type = new LatinAdj(splitresults[0],
                                splitresults[1], entrypos == PoS.Num);
                        }
                        else {
                            if ((splitresults[1] == "a") &
                                (splitresults[2] == "um"))
                                if (splitresults[0].EndsWith("s"))
                                {
                                    splitresults[1] = splitresults[0]
                                        .Substring(0,
                                        splitresults[0].Length - 2) + "ă";
                                    splitresults[2] = splitresults[0]
                                        .Substring(0,
                                        splitresults[0].Length - 2) + "um";
                                }
                                else
                                {
                                    splitresults[1] = splitresults[0] + "ă";
                                    splitresults[2] = splitresults[0] + "um";
                                }
                            type = new LatinAdj(splitresults[0],
                                splitresults[1], splitresults[2],
                                entrypos == PoS.Num);
                        }
                        break;
                }
            }
            else
            { type = null; pos = PoS.Indecl; }
        }
        public Entry(LatinNoun.Gend g, string[] splitresults)
        {
            if (splitresults.Length == 0)
            {
                type = null;
                pos = PoS.Indecl;
            }
            else
            {
                string nom = splitresults[0];
                string gen;
                if (splitresults.Length == 1) gen = splitresults[0];
                else gen = splitresults[1];
                if (gen != nom)
                {
                    string stem = nom.Substring(0, nom.Length - 2);
                    if (Pr.IsLatinVowel(nom[nom.Length-1]) &
                        !Pr.EW(nom, "ae"))
                        stem = stem + nom[nom.Length - 2];
                    else if (Pr.Delen(gen) == "is")
                    {
                        if (!Pr.EW(nom, "s"))
                            stem = nom;
                        gen = stem + gen;
                    }
                    if ((Pr.Delen(gen) == "ae") | (Pr.Delen(gen) == "ei")
                        | (Pr.Delen(gen) == "us") | (Pr.Delen(gen) ==
                        "arum") | (Pr.Delen(gen) == "uum") | (Pr.Delen(gen) 
                        == "orum") | (Pr.Delen(gen) == "onis") | 
                        (Pr.Delen(gen) == "erum") | (Pr.Delen(gen) == "erum")) 
                        gen = stem + gen;
                    if (Pr.Delen(gen) == "i")
                    {
                        if (!Pr.EW(nom, "s") & !Pr.EW(nom, "m"))
                            stem = nom;
                        gen = stem + gen;
                    }
                }
                type = new LatinNoun(nom, gen, g);
                pos = PoS.Noun;
            }
        }
        public Entry(LatinNoun n) { pos = PoS.Noun; type = n; }
        public Entry(LatinAdj a)
        {
            type = a;
            if (a.NumerusCardinalisNecUnus) pos = PoS.Num;
            else pos = PoS.Adj;
        }
        public Entry(LatinVerb v) { pos = PoS.Verb; type = v; }
        public Entry(string indecl) { pos = PoS.Indecl; type = indecl; }
        public string Inflect(bool independent = true)
        {
            if (type == null) return "";
            switch (pos)
            {
                case PoS.Indecl:
                    if (independent)
                        return type.ToString() + "\nIndeclinabile.";
                    else return type.ToString();
                case PoS.Noun:
                    LatinNoun x = (LatinNoun)type;
                    return x.Inflect(independent);
                case PoS.Verb:
                    LatinVerb y = (LatinVerb)type;
                    return y.Inflect(independent);
                default:
                    LatinAdj z = (LatinAdj)type;
                    return z.Inflect(independent);
            }
        }
    }
    struct LatinVerb
    {
        public bool EndsWith(string s)
        {
            return Pr.Delen(BaseForm).EndsWith(s);
        }
        public static readonly List<string> TenseNames =
            new List<string> { "Praesens", "Imperfectum", "Futurum primum", 
               "Perfectum", "Plusquamperfectum", "Futurum secundum" };
        public static readonly List<string> MoodNames = 
            new List<string> { "Infinitivus", "Imperativus",
                "Coniunctivus", "Indicativus" };
        static LatinVerb esse = new LatinVerb("sum", "fuī", "futurus", "essē");
        public static LatinVerb Esse
        {
            get { return esse; }
        }
        bool intrans;
        public bool Intrans
        {
            get
            {
                if (deponensPrs) return true;
                if (deponensPfct) return true;
                if (conjug == Conjug.Sum) return true;
                if (Pfct1Sg.EndsWith("t")) return true;
                return intrans;
            }
            set
            {
                if (value) intrans = true;
                else if (!deponensPfct & !deponensPrs)
                    intrans = (conjug == Conjug.Sum) | Pfct1Sg.EndsWith("t");
            }
        }
        bool CapitalFirst; //words stored in toLower
        string Prs1Sg;
        public string BaseForm
        {
            get
            {
                if (Prs1Sg == "") return Pfct1Sg;
                return Prs1Sg;
            }
        }
        string Pfct1Sg;
        internal string SupinStem;
        string PrsInf;
        string EsseConjPrs(byte p, bool plural)
        {
            switch (p % 3)
            {
                case 1: if (plural) return "sīmŭs"; else return "sim";
                case 2: if (plural) return "sītĭs"; else return "sīs";
                default: if (plural) return "sint"; else return "sit";
            }
        }
        string Prs3Pl(bool passive)
        {
            if (Prs1Sg == "") return "";
            if (EndsWith("t") | EndsWith("tur")) return Prs1Sg;
            string stem;
            if (deponensPrs)
                    stem = Prs1Sg.Substring(0, Prs1Sg.Length - 2);
            else
                stem = Prs1Sg.Substring(0, Prs1Sg.Length - 1);
            if (conjug == Conjug.A)
                return stem + AmIndConjugation(3, true, passive);
            else if ((conjug == Conjug.Sum) | (conjug == Conjug.E))
                if (passive)
                    return stem + "ntur";
                else
                    return stem + "nt";
            else
                return stem + ConsIndConjugation(3, true, passive);
        }
        static bool isFuture(Tense t)
        {
            return (int)t % 3 == 2;
        }
        static bool isImpfTense(Tense t)
        {
            return (int)t % 3 == 1;
        }
        static bool isPfctSystem(Tense t)
        {
            return t >= Tense.Pfct;
        }
        bool deponensPrs
        {
            get
            {
                return Prs1Sg.EndsWith("r");
            }
        }
        bool deponensPfct
        {
            get
            {
                return Pfct1Sg.Contains(" ");
            }
        }
        public bool Deponens
        {
            get
            {
                return deponensPrs & deponensPfct;
            }
        }
        public bool Semideponens
        {
            get
            {
                if (Deponens) return false;
                return deponensPrs | deponensPfct;
            }
        }
        public enum Tense { Prs = 0, Impf = 1, FutI = 2, Pfct = 3, Plpf = 4, FutII = 5 };
        public enum Mood { Inf = 0, Impv = 1, Conj = 2, Ind = 3 };
        enum Conjug { Irreg = 0, A = 1, E = 2, Cons = 3, I = 4, Capio = 5, Sum = 6, Eo = 7 };
        Conjug conjug;
        public string Conjugation
        {
            get
            {
                switch (conjug)
                {
                    case Conjug.A: return "1";
                    case Conjug.E: return "2";
                    case Conjug.Cons: return "3";
                    case Conjug.Capio: return "3a";
                    case Conjug.I: return "4";
                    default: return "irregularis";
                }
            }
        }
        string Pfct3Sg
        {
            get
            {
                if (Pr.EW(Pfct1Sg, "i"))
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 1) + "it";
                else if (deponensPfct)//Verba (semi)deponentia
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 3) + "est";
                else return Pfct1Sg; //Verba defectiva
            }
        }
        string Pfct1Pl
        {
            get
            {
                if (Pr.EW(Pfct1Sg, "i"))
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 1) + "ĭmŭs";
                else if (Pfct1Sg.EndsWith("t") | (Pfct1Sg.Length < 2))
                    return Pfct1Sg; //Verba defectiva
                else //Verba (semi)deponentia
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 2) + "ŭmŭs";
            }
        }
        string ConsPfctStem
        {
            get
            {
                if (deponensPfct) return SupinStem;
                if (Pfct1Sg.EndsWith("t"))
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 2) + "is";
                return Pfct1Sg.Substring(0, Pfct1Sg.Length - 1) + "is";
            }
        }
        string VocPfctStem
        {
            get
            {
                if (deponensPfct) return SupinStem;
                if (Pfct1Sg.EndsWith("t"))
                    return Pfct1Sg.Substring(0, Pfct1Sg.Length - 2) + "ĕr";
                return Pfct1Sg.Substring(0, Pfct1Sg.Length - 1) + "ĕr";
            }
        }
        public bool NoSupins;
        public string Supinum1
        {
            get
            {
                if ((SupinStem == "") | NoSupins) return "";
                return SupinStem + "um";
            }
        }
        public string Supinum2
        {
            get
            {
                if ((SupinStem == "") | NoSupins) return "";
                return SupinStem + "ū";
            }
        }
        public LatinAdj PPP
        {
            get
            {
                if (SupinStem == "") return new LatinAdj("", "");
                LatinAdj ans = new LatinAdj(SupinStem + "ŭs",
                    SupinStem + "ă", SupinStem + "um");
                ans.Participium = true;
                return ans;
            }
        }
        public LatinAdj PFA
        {
            get
            {
                if (PFAStem == "") return new LatinAdj("", "");
                LatinAdj ans = new LatinAdj(PFAStem + "ūrŭs",
                    PFAStem + "ūră", PFAStem + "ūrum");
                ans.Participium = true;
                return ans;
            }
            set
            {
                string s = value.Nominative;
                if (s.Length > 4)
                    PFAStem = s.Substring(0, s.Length - 4);
            }
        }
        internal string PFAStem;
        string InfFuturi(bool passive)
        {
            if (passive)
            {
                if (Supinum1 == "") return "";
                else return Supinum1 + " īrī";
            }
            else
            {
                if (PFAStem == "") return "";
                string ans = PFA.OneForm(LatinNoun.Case.Acc, LatinNoun.Gend.N) + " esse";
                if (Pr.EW(PrsInf, "esse"))
                    ans += "/" + PrsInf.Replace("essē", "fŏrē");
                return ans.ToLower();
            }
        }
        string addstem
        {
            get
            {
                if (Pr.Delen(PrsInf) == "posse") return "pote";
                string stem = "";
                switch (conjug)
                {
                    case Conjug.Sum://praesens, absens
                        stem = Prs1Sg.Substring(0, PrsInf.Length - 4) + "se";
                        break;
                    case Conjug.Cons:
                        stem = Prs1Sg.Substring(0, Prs1Sg.Length - 1) + "e";
                        break;
                    case Conjug.A:
                        stem = PrsInf.Substring(0, PrsInf.Length - 3) + "a";
                        break;
                    case Conjug.E:
                        stem = PrsInf.Substring(0, PrsInf.Length - 3) + "e";
                        break;
                    case Conjug.Eo:
                        stem = PrsInf.Substring(0, PrsInf.Length - 3) + "ĕu";
                        break;
                    case Conjug.I:
                        stem = PrsInf.Substring(0, PrsInf.Length - 3) + "ie";
                        break;
                    case Conjug.Capio:
                        if (EndsWith("ior"))
                            stem = Prs1Sg.Substring(0, Prs1Sg.Length - 2) + "e";
                        else if (EndsWith("itur"))
                            stem = Prs1Sg.Substring(0, Prs1Sg.Length - 3) + "e";
                        else stem = Prs1Sg.Substring(0, Prs1Sg.Length - 1) + "e";
                        break;
                    default:
                        stem = PrsInf.Substring(0, PrsInf.Length - 2) + "e";
                        break;
                }
                if ((Pr.Delen(stem) == "vele") & (conjug == Conjug.Irreg))
                    stem = "vŏle"; //for "velle"
                return stem;
            }
        }
        public override string ToString()
        {
            string ans;
            if (Prs1Sg == "")
                if (SupinStem == "") ans = Pfct1Sg + ", " +
                        ConsPfctStem + "sē";
                else ans = Pfct1Sg + ", " + SupinStem + "ūrŭs, " +
                        ConsPfctStem + "sē";
            else if (deponensPfct | deponensPrs)
                ans = Prs1Sg + ", " + Pfct1Sg + ", " + PrsInf;
            else
            {
                if (SupinStem == "") ans = "-";
                else if (NoSupins) ans = SupinStem + "ūrŭs";
                else ans = SupinStem + "um";
                ans = Prs1Sg + ", " + Pfct1Sg + ", " + ans + ", " + PrsInf;
            }
            ans += "\nVerbum";
            if (Deponens) ans += " deponens";
            else if (Semideponens) ans += " semideponens";
            if ((Prs1Sg == "") | Pfct1Sg.EndsWith("t"))
                ans += " defectivum";
            ans += ".\nConiugatio " + Conjugation + ".";
            return ans;
        }
        public LatinAdj PPA
        {
            get
            {
                LatinAdj ans;
                ans = new LatinAdj(addstem + "ns", addstem + "ntis");
                if (conjug == Conjug.Eo) ans = new LatinAdj(addstem
                    .Substring(0, addstem.Length - 2) + "iens",
                    addstem + "ntis");
                if (Pr.Delen(PrsInf) == "esse") ans = new LatinAdj("ens", "entis");
                ans.Participium = true;
                return ans;
            }
        }
        public LatinAdj Gerundivum
        {
            get
            {
                return new LatinAdj(addstem + "ndŭs", addstem + "ndă",
                    addstem + "ndum");
            }
        }
        public LatinNoun Gerundium
        {
            get
            {
                LatinNoun ger = new LatinNoun(PrsInf, addstem + "ndī",
                    LatinNoun.Gend.N);
                ger.NoPlural = true;
                ger.AddVariantForm(LatinNoun.Case.Acc, false, addstem + "ndum");
                return ger;
            }
        }
        string ConsIndConjugation(byte p, bool plural, bool passive)
        {
            switch (p % 3)
            {
                case 1:
                    if (passive)
                        if (plural) return "ĭmur"; else return "or";
                    else if (plural) return "ĭmŭs"; else return "ō";
                case 2:
                    if (passive)
                        if (plural) return "ĭmĭnĭ"; else return "ĕrĭs";
                    else if (plural) return "ĭtĭs"; else return "ĭs";
                default:
                    if (passive)
                        if (plural) return "untur"; else return "ĭtur";
                    else if (plural) return "unt"; else return "it";
            }
        }
        string AmIndConjugation(byte p, bool plural, bool passive)
        {
            switch (p % 3)
            {
                case 1:
                    if (passive)
                        if (plural) return "āmur"; else return "ar";
                    else if (plural) return "āmŭs"; else return "am";
                case 2:
                    if (passive)
                        if (plural) return "āmĭnĭ"; else return "ārĭs";
                    else if (plural) return "ātĭs"; else return "ās";
                default:
                    if (passive)
                        if (plural) return "antur"; else return "ātur";
                    else if (plural) return "ant"; else return "at";
            }
        }
        string EmIndConjugation(byte p, bool plural, bool passive)
        {
            switch (p % 3)
            {
                case 1:
                    if (passive)
                        if (plural) return "ēmur"; else return "er";
                    else if (plural) return "ēmŭs"; else return "em";
                case 2:
                    if (passive)
                        if (plural) return "ēmĭnĭ"; else return "ērĭs";
                    else if (plural) return "ētĭs"; else return "ēs";
                default:
                    if (passive)
                        if (plural) return "entur"; else return "ētur";
                    else if (plural) return "ent"; else return "et";
            }
        }
        Dictionary<Tuple<Tense, Mood, byte, bool, bool>, string>
            irregularForms;
        public void AddIrregularForm(Tense t, Mood m, byte p, bool plural, bool passive, string s)
        {
            s = s.ToLower();
            Tuple<Tense, Mood, byte, bool, bool> form = new Tuple<Tense,
                Mood, byte, bool, bool>(t, m, p, plural, passive);
            if (!irregularForms.ContainsKey(form)) irregularForms.Add(form, s);
            else {
                if ((irregularForms[form] != s) &
                    !irregularForms[form].StartsWith(s + "/") &
                    !irregularForms[form].Contains("/" + s))
                    if (irregularForms[form] == "") irregularForms[form] = s;
                    else
                        irregularForms[form] = irregularForms[form] + "/" + s;
            }
        }
        public void AddVariantForm(Tense t, Mood m, byte p, bool plural, bool passive, string s)
        {
            Tuple<Tense, Mood, byte, bool, bool> form = new Tuple<Tense,
                Mood, byte, bool, bool>(t, m, p, plural, passive);
            if (!irregularForms.ContainsKey(form))
                irregularForms[form] = GetVerbalForm(t, m, p, plural, passive);
            AddIrregularForm(t, m, p, plural, passive, s);
        }
        public void SetLacuna(Tense t, Mood m, byte p, bool plural, bool passive)
        {
            Tuple<Tense, Mood, byte, bool, bool> form = new Tuple<Tense,
                Mood, byte, bool, bool>(t, m, p, plural, passive);
            irregularForms[form] = "";
        }
        string GetVerbalForm(Tense t, Mood m, byte p, bool plural, bool passive)
        {
            if ((m == Mood.Conj) & isFuture(t)) t -= 2;
            if (Pr.EW(PrsInf, "lle") & (t == Tense.Prs)) passive = false;
            if (m == Mood.Impv)
            {
                if (isPfctSystem(t)) t -= 3;
                if (t == Tense.Impf) t = Tense.Prs;
                if (p == 1) m = Mood.Conj;
                if (p == 0) t = Tense.FutI;
                if ((p == 2) & passive & (t == Tense.Prs)) m = Mood.Ind;
            }
            if ((!isPfctSystem(t) & deponensPrs) |
                (isPfctSystem(t) & deponensPfct)) passive = true;
            Tuple<Tense, Mood, byte, bool, bool> form = new Tuple<Tense,
                Mood, byte, bool, bool>(t, m, p, plural, passive);
            if (irregularForms.ContainsKey(form)) return irregularForms[form];
            if (Pfct1Sg.EndsWith("t")) p = 0;
            if (conjug == Conjug.Sum)
            {
                passive = false;
                if ((m == Mood.Conj) & (t == Tense.Prs))
                    return Prs1Sg.Substring(0, Prs1Sg.Length - 3) +
                        EsseConjPrs(p, plural); //sic, cf. prosum, possum
            }
            if (m == Mood.Inf)
            {
                plural = false; p = 0;
                if (isFuture(t)) return InfFuturi(passive);
                if (isImpfTense(t)) t -= 1;
                if (t == Tense.Prs & (!passive | deponensPrs)) return PrsInf;
                if (t == Tense.Pfct)
                    if (passive)
                        if (SupinStem == "") return "";
                        else return SupinStem + "um essē";
                    else return ConsPfctStem + "sē";
                if ((conjug == Conjug.Cons) | (conjug == Conjug.Capio))
                    return PrsInf.Substring(0, PrsInf.Length - 3) + "ī";
                else
                    return PrsInf.Substring(0, PrsInf.Length - 1) + "ī";
            }
            string currstem;
            if ((m == Mood.Ind) & (t == Tense.Prs))
            {
                if ((p == 0) & plural) return Prs3Pl(passive);
                currstem = Prs1Sg.Substring(0, Prs1Sg.Length - 1);
                if ((p == 1) & !plural)
                    if (!passive | deponensPrs) return Prs1Sg;
                    else if (Prs1Sg.EndsWith("m")) return currstem + "r";
                    else return currstem + "or";
                switch (conjug)
                {
                    case Conjug.Sum:
                        if (p == 1) return Prs1Sg.Substring(0,
                            Prs1Sg.Length - 2) + "ŭmŭs";
                        currstem = PrsInf.Substring(0, PrsInf.Length - 2);
                        if (currstem == "pos") currstem = "potĕs";
                        if (p == 0) return currstem + "t";
                        else if (plural) return currstem + "tĭs";
                        else return currstem;
                    case Conjug.A:
                        return currstem + AmIndConjugation(p, plural, passive);
                    case Conjug.Cons:
                        return currstem + ConsIndConjugation(p, plural, passive);
                    case Conjug.E:
                        return currstem.Substring(0, currstem.Length - 1)
                            + EmIndConjugation(p, plural, passive);
                    case Conjug.Capio:
                        return currstem.Substring(0, currstem.Length - 1)
                            + ConsIndConjugation(p, plural, passive);
                    case Conjug.Irreg:
                        if (Pr.EW(PrsInf, "lle"))
                        {
                            if (p == 1) return currstem + "ŭmŭs";
                            if (Pr.Delen(PrsInf) == "nolle")
                                currstem = "nōn v";
                            else if (Pr.Delen(currstem) == "mal")
                                currstem = "mav";
                            else currstem = "v";
                            if ((p == 2) & !plural) return currstem + "īs";
                            currstem = currstem + "ul";
                        }
                        if (p == 0) if (passive) return currstem + "tur";
                            else return currstem + "t";
                        else if (p == 1)
                            if (passive) return currstem + "ĭmur";
                            else return currstem + "ĭmŭs";
                        else if (plural)
                            if (passive) return currstem + "ĭmĭnĭ";
                            else return currstem + "tĭs";
                        else if (passive) return currstem + "rĭs";
                        else return currstem + "s";
                    default: //case Conjug.I: & case Conjug.Eo;
                        currstem = currstem.Substring(0, currstem.Length - 1);
                        if ((p == 0) & !passive) return currstem + "it";
                        currstem = currstem + "ī";
                        if (p == 1) if (passive) return currstem + "mur";
                            else return currstem + "mŭs";
                        if (p == 0) return currstem + "tur";
                        if (plural) //Necessarily 2nd person
                            if (passive) return currstem + "mĭnĭ";
                            else return currstem + "tĭs";
                        else if (passive) return currstem + "rĭs";
                        else return currstem + "s";
                }
            }
            if (isPfctSystem(t) & passive)
            {
                if (Pfct1Sg.EndsWith("t")) return SupinStem + "um " +
                         esse.GetVerbalForm(t - 3, m, 0, false, false);
                if (!plural) return SupinStem + "ŭs/-ă/-um " +
                        esse.GetVerbalForm(t - 3, m, p, false, false);
                else return SupinStem + "ī/-ae/-ă " +
                        esse.GetVerbalForm(t - 3, m, p, true, false);
            }
            if ((t == Tense.Pfct) & (m == Mood.Ind))
                if (p == 2)
                    if (plural) return ConsPfctStem + "tīs";
                    else return ConsPfctStem + "tī";
                else if (!plural)
                    if (p == 1) return Pfct1Sg; else return Pfct3Sg;
                else if ((p == 1) | (Pfct1Sg == Pfct3Sg)) return Pfct1Pl;
                else
                    return Pfct3Sg.Substring(0, Pfct3Sg.Length - 2) + "ērē/"
                        + Pfct3Sg.Substring(0, Pfct3Sg.Length - 2) + "ērunt";
            if ((t == Tense.Plpf) & (m == Mood.Ind))
                return VocPfctStem + AmIndConjugation(p, plural, false);
            if ((t == Tense.Plpf) & (m == Mood.Conj))
                return ConsPfctStem + "s" + EmIndConjugation(p, plural, false);
            if ((t == Tense.Pfct) & (m == Mood.Conj) & (p == 1) & !plural)
                return VocPfctStem + "im";
            if (isPfctSystem(t))
                return VocPfctStem + ConsIndConjugation(p, plural, false);
            if (PrsInf == "") return "";
            currstem = PrsInf.Substring(0, PrsInf.Length - 2);
            if (Pr.EW(PrsInf, "i"))
                if (Pr.Delen(PrsInf) == "fieri")
                    currstem = "fī";
                else if (conjug == Conjug.Capio) //orior
                    currstem = Prs1Sg.Substring(0, Prs1Sg.Length - 3) + "ĕ";
                else if (conjug == Conjug.Cons) //sequor
                    currstem = Prs1Sg.Substring(0, Prs1Sg.Length - 2) + "ĕ";
            if (m == Mood.Impv)
            {
                if (t == Tense.FutI)
                {
                    if (Pr.EW(currstem, "e") & (conjug != Conjug.E))
                        currstem = currstem.Substring(0,
                            currstem.Length - 1) + "ĭ";
                    if (plural)
                        if (p == 0) if (passive) return Prs3Pl(false) + "or";
                            else return Prs3Pl(false) + "ō";
                        else if (passive) return currstem + "mĭnō";
                        else return GetVerbalForm(t, m, 2, false, false) + "tē";
                    else if (passive) return currstem + "tor";
                    else return currstem + "tō";
                }
                else if (Pr.Delen(PrsInf) == "nolle")
                    if (plural) return "nolītē"; else return "nolī";
                else if (Pr.Delen(PrsInf) == "fieri" | Pr.EW(PrsInf, "velle")
                       | Pr.EW(PrsInf, "malle")) return "";
                else if (!plural)
                    if (passive)
                        return currstem + "rĕ";
                    else if (Pr.EW(PrsInf, "dicere") | Pr.EW(PrsInf, "ducere")
                        | Pr.EW(PrsInf, "facere") | Pr.EW(PrsInf, "ficere"))
                        return PrsInf.Substring(0, PrsInf.Length - 3);
                    else return currstem;
                else //Impv. Prs. Act. Pl.: pass was sent to Ind.
                {
                    if (Pr.EW(currstem, "e") & (conjug != Conjug.E))
                        currstem = currstem.Substring(0, currstem.Length - 1) + "ĭ";
                    return currstem + "tē";
                }
            }
            if (conjug == Conjug.Sum)
            {
                if (Pr.Delen(PrsInf) == "posse") currstem = "potĕr";
                else currstem = currstem.Substring(0,
                    currstem.Length - 1) + "r";
            }
            if (t == Tense.Impf)
                if (m == Mood.Conj)
                    if (Pr.EW(PrsInf, "i"))
                        return currstem + "r" +
                            EmIndConjugation(p, plural, passive);
                    else //Not to have "esrem" pro "essem" vel sim.
                        return PrsInf.Substring(0, PrsInf.Length - 1) +
                            EmIndConjugation(p, plural, passive);
                else
                {
                    if (conjug == Conjug.Sum)
                        return currstem + AmIndConjugation(p, plural, false);
                    if (Pr.EW(currstem, "e") & (conjug != Conjug.E))
                        currstem = addstem.Substring(0, currstem.Length - 1) + "ē";
                    if (Pr.Delen(PrsInf) == "fieri" | conjug == Conjug.I) currstem += "ē";
                    return currstem + "b" + AmIndConjugation(p, plural, false);
                }
            if ((t == Tense.FutI) & ((conjug == Conjug.A) |
                (conjug == Conjug.E) | (conjug == Conjug.Eo)))
                return currstem + "b" + ConsIndConjugation(p, plural, passive);
            if (Pr.EW(currstem, "e") & (conjug != Conjug.E))
                currstem = currstem.Substring(0, currstem.Length - 1) + "ĭ";
            switch (conjug)
            {
                case Conjug.A:
                    return currstem.Substring(0, currstem.Length - 1)
                            + EmIndConjugation(p, plural, passive);
                case Conjug.E:
                    return currstem.Substring(0, currstem.Length - 1) + "e"
                            + AmIndConjugation(p, plural, passive);
                case Conjug.Eo:
                    return currstem.Substring(0, currstem.Length - 1) + "e"
                            + AmIndConjugation(p, plural, passive);
                case Conjug.Sum: //Futurum as conj was dealt with before.
                    return currstem + ConsIndConjugation(p, plural, false);
                default:
                    if (Pr.EW(currstem, "i")) currstem =
                            currstem.Substring(0, currstem.Length - 1) + "i";
                    if (((p != 1) | plural) & (t == Tense.FutI))
                        return currstem + EmIndConjugation(p, plural, passive);
                    else return currstem + AmIndConjugation(p, plural, passive);
            }
        }
        public string OneForm(Tense t, Mood m, byte p, bool plural, bool passive)
        {
            string ans = GetVerbalForm(t, m, (byte)(p % 3), plural, passive);
            if (CapitalFirst)
                ans = ans[0].ToString().ToUpper() + ans.Substring(1);
            return ans;
        }
        public LatinVerb(string sg1, string pfct, string sup, string inf)
        {
            irregularForms = new Dictionary<Tuple
                <Tense, Mood, byte, bool, bool>, string>();
            NoSupins = false;
            if (sup.Length <= 2) PFAStem = SupinStem = "";
            else if (Pr.EW(sup, "urus"))
            {
                PFAStem = sup.Substring(0, sup.Length - 4);
                SupinStem = "";
                NoSupins = true;
            }
            else PFAStem = SupinStem = sup.Substring(0, sup.Length - 2);
            Prs1Sg = sg1;
            PrsInf = inf;
            Pfct1Sg = pfct;
            CapitalFirst = Prs1Sg[0].ToString().ToLower()[0] != Prs1Sg[0];
            if (Pr.EW(inf, "are") | Pr.EW(inf, "ari")) conjug = Conjug.A;
            else if (Pr.EW(inf, "ire") | Pr.EW(inf, "iri"))
            {
                if (Pr.EW(sg1, "eo") | Pr.EW(sg1, "eor")) conjug = Conjug.Eo;
                else
                    conjug = Conjug.I;
            }
            else if (Pr.EW(sg1, "eo") | Pr.EW(sg1, "eor") |
                    Pr.EW(sg1, "et") | Pr.EW(sg1, "etur")) conjug = Conjug.E;
            else if (Pr.EW(sg1, "io") | Pr.EW(sg1, "ior")) conjug = Conjug.Capio;
            else
            {
                conjug = Conjug.Cons;
                if (Pr.EW(inf, "lle")) conjug = Conjug.Irreg;
                if (Pr.EW(inf, "ferre")) conjug = Conjug.Irreg;
                if (Pr.EW(inf, "esse")) conjug = Conjug.Sum;
                if (Pr.EW(inf, "posse")) conjug = Conjug.Sum;
            }
            intrans = (conjug == Conjug.Sum) | Prs1Sg.EndsWith("t") |
                Prs1Sg.EndsWith("r") | Pfct1Sg.Contains(" ");
            LengthCorrector();
        }
        public LatinVerb(string sg1, string pfct, string inf)
        {
            irregularForms = new Dictionary<Tuple
                <Tense, Mood, byte, bool, bool>, string>();
            Prs1Sg = sg1;
            PrsInf = inf;
            Pfct1Sg = pfct;
            CapitalFirst = Prs1Sg[0].ToString().ToLower()[0] != Prs1Sg[0];
            NoSupins = false;
            if (pfct.Contains(" "))
                SupinStem = pfct.Substring(0, pfct.IndexOf(' ') - 2);
            else SupinStem = "";
            if (Pr.EW(inf, "are") | Pr.EW(inf, "ari")) conjug = Conjug.A;
            else if (Pr.EW(inf, "ire") | Pr.EW(inf, "iri"))
            {
                if (Pr.EW(sg1, "eo") | Pr.EW(inf, "eor")) conjug = Conjug.Eo;
                else
                    conjug = Conjug.I;
            }
            else if (Pr.EW(sg1, "eo") | Pr.EW(sg1, "eor") |
                    Pr.EW(sg1, "et") | Pr.EW(sg1, "etur")) conjug = Conjug.E;
            else if (Pr.EW(sg1, "io") | Pr.EW(sg1, "ior")) conjug = Conjug.Capio;
            else
            {
                conjug = Conjug.Cons;
                if (Pr.EW(inf, "lle")) conjug = Conjug.Irreg;
                if (Pr.EW(inf, "ferre")) conjug = Conjug.Irreg;
                if (Pr.EW(inf, "esse")) conjug = Conjug.Sum;
                if (Pr.EW(inf, "posse")) conjug = Conjug.Sum;
            }
            intrans = (conjug == Conjug.Sum) | Prs1Sg.EndsWith("t") |
                Prs1Sg.EndsWith("r") | Pfct1Sg.Contains(" ");
            PFAStem = SupinStem;
            if (Pr.EW(pfct, "urus")) SupinStem = "";
            LengthCorrector();
        }
        void LengthCorrector()
        {
            if (PrsInf != "")
            {
                if (Pr.EW(PrsInf, "ere"))
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 3) + "ĕ"
                        + PrsInf.Substring(PrsInf.Length - 2);
                if (conjug == Conjug.E) //rewrites previous step
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 3) + "ē"
                        + PrsInf.Substring(PrsInf.Length - 2);
                if (conjug == Conjug.A)
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 3) + "ā"
                        + PrsInf.Substring(PrsInf.Length - 2);
                if ((conjug == Conjug.I) | (conjug == Conjug.Eo))
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 3) + "ī"
                        + PrsInf.Substring(PrsInf.Length - 2);
                if (Pr.EW(PrsInf, "i"))
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 1) + "ī";
                if (Pr.EW(PrsInf, "e"))
                    PrsInf = PrsInf.Substring(0, PrsInf.Length - 1) + "ē";
            }
            if ((Prs1Sg != "") & (EndsWith("o")))
                Prs1Sg = Prs1Sg.Substring(0, Prs1Sg.Length - 1) + "ō";
            if (Pr.EW(Pfct1Sg, "i"))
                Pfct1Sg = Pfct1Sg.Substring(0, Pfct1Sg.Length - 1) + "ī";
        }
        public LatinVerb(string pfctonly, string ptcp = "")
        {
            irregularForms = new Dictionary<Tuple
                <Tense, Mood, byte, bool, bool>, string>();
            CapitalFirst = pfctonly[0].ToString().ToLower()[0] != pfctonly[0];
            Prs1Sg = "";
            Pfct1Sg = pfctonly;
            PrsInf = "";
            NoSupins = true;
            if (Pr.EW(ptcp, "urus"))
            {
                PFAStem = ptcp.Substring(0, ptcp.Length - 4);
                SupinStem = "";
            }
            else if (Pr.EW(ptcp, "us"))
                PFAStem = SupinStem = ptcp.Substring(0, ptcp.Length - 2);
            else PFAStem = SupinStem = "";
            conjug = Conjug.Irreg;
            intrans = true;
            LengthCorrector();
        }
        string Conjugate(Tense t, Mood m, bool passive = false)
        {
            if (isPfctSystem(t) & !passive & (Pfct1Sg.Length < 2))
                return "Non habet.\n";
            if ((m == Mood.Inf) | Pfct1Sg.EndsWith("t"))
                return OneForm(t, m, 0, false, passive);
            string ans = "";
            if (m == Mood.Impv)
                if (Prs1Sg == "")
                {
                    t = Tense.FutI;
                    if (!irregularForms.ContainsKey(new Tuple<Tense, Mood,
                        byte, bool, bool>(t, m, 2, false, false))) return "";
                    else ans += "\nPersona secunda et tertia singularis: " +
                            OneForm(t, m, 2, false, false) +
                            "\nPersona secunda pluralis: " +
                            OneForm(t, m, 2, true, false);
                }
                else if (t == Tense.Prs)
                    ans += "\nPersona 2 singularis:\t" + OneForm(t, m, 2,
                        false, passive) + "\nPersona 2 pluralis:\t" +
                        OneForm(t, m, 2, true, passive);
                else {
                    for (byte p = 2; p < 4; p++)
                    {
                        ans += "\nPersona " + p.ToString() + " singularis:\t" +
                            OneForm(t, m, p, false, passive);
                    }
                    for (byte p = 2; p < 4; p++)
                    {
                        ans += "\nPersona " + p.ToString() + " pluralis:\t" +
                            OneForm(t, m, p, true, passive);
                    }
                }
            else {
                for (byte p = 1; p < 4; p++)
                {
                    ans += "\nPersona " + p.ToString() + " singularis:\t" +
                        OneForm(t, m, p, false, passive);
                }
                for (byte p = 1; p < 4; p++)
                {
                    ans += "\nPersona " + p.ToString() + " pluralis:\t" +
                        OneForm(t, m, p, true, passive);
                }
            }
            return ans + '\n';
        }
        public string Inflect(bool independent = true)
        {
            string ans = "";
            if (independent) ans = ToString() + '\n';
            if (Prs1Sg == "")
            {
                ans += "Infinitivus:\t" + Conjugate(Tense.Pfct, Mood.Inf) + ".\n";
                if (SupinStem != "")
                {
                    ans += "Participium futuri:\n" + PFA.Inflect(false);
                }
                ans += "Perfectum-praesens indicativi: " +
                    Conjugate(Tense.Pfct, Mood.Ind);
                ans += "Plusquamperfectum indicativi: " +
                    Conjugate(Tense.Plpf, Mood.Ind);
                ans += "Futurum indicativi: " +
                    Conjugate(Tense.FutII, Mood.Ind);
                ans += "Perfectum-praesens coniunctivi: " +
                    Conjugate(Tense.Pfct, Mood.Conj);
                ans += "Plusquamperfectum coniunctivi: " +
                    Conjugate(Tense.Plpf, Mood.Conj);
                string impv = Conjugate(Tense.FutII, Mood.Impv);
                if (impv != "") ans += "Imperativus: " + impv;
            }
            else {
                ans += "\nGerundium:\n" + Gerundium.Inflect(false);
                ans += "Gerundivum:\n" + Gerundivum.Inflect(false);
                ans += "Infinitivi:\n";
                if (Intrans)
                {
                    ans += "praesentis:\t" + Conjugate(Tense.Prs, Mood.Inf, deponensPrs);
                    ans += ";\nperfecti:\t" + Conjugate(Tense.Pfct, Mood.Inf, deponensPfct);
                    if (SupinStem != "")
                    {
                        ans += ";\nfuturi:\t" + Conjugate(Tense.FutI, Mood.Inf);
                        //For verba deponentia, inf. futuri always with PFA
                        if (NoSupins) ans += "Supina non habet.\n";
                        else
                        {
                            ans += "\nSupinum primum:  \t" + Supinum1;
                            ans += "\nSupinum secundum:\t" + Supinum2 + "\n";
                        }
                        if (deponensPfct)
                        {
                            ans += "\nParticipium perfecti:\n" + PPP.Inflect(false);
                        }
                        ans += "\nParticipium futuri:\n" + PFA.Inflect(false);
                    }
                    else if (PFAStem != "")
                    {
                        ans += ";\nfuturi:  \t" + Conjugate(Tense.FutI, Mood.Inf);
                        ans += "\nParticipium futuri:\n" + PFA.Inflect(false);
                    }
                    else ans += "\n\n";
                    ans += "Participium praesentis:\n" + PPA.Inflect(false);
                    ans += "Praesens indicativi: " +
                        Conjugate(Tense.Prs, Mood.Ind, deponensPrs);
                    ans += "\nImperfectum indicativi: " +
                        Conjugate(Tense.Impf, Mood.Ind, deponensPrs);
                    ans += "\nFuturum indicativi: " +
                        Conjugate(Tense.FutI, Mood.Ind, deponensPrs);
                    ans += "\nPerfectum indicativi: " +
                        Conjugate(Tense.Pfct, Mood.Ind, deponensPfct);
                    ans += "\nPlusquamperfectum indicativi: " +
                        Conjugate(Tense.Plpf, Mood.Ind, deponensPfct);
                    ans += "\nFuturum secundum indicativi: " +
                        Conjugate(Tense.FutII, Mood.Ind, deponensPfct);
                    ans += "\nPraesens coniunctivi: " +
                        Conjugate(Tense.Prs, Mood.Conj, deponensPrs);
                    ans += "\nImperfectum coniunctivi: " +
                        Conjugate(Tense.Impf, Mood.Conj, deponensPrs);
                    ans += "\nPerfectum coniunctivi: " +
                        Conjugate(Tense.Pfct, Mood.Conj, deponensPfct);
                    ans += "\nPlusquamperfectum coniunctivi: " +
                        Conjugate(Tense.Plpf, Mood.Conj, deponensPfct);
                    ans += "\nImperativus praesentis: " +
                        Conjugate(Tense.Prs, Mood.Impv, deponensPrs);
                    ans += "\nImperativus futuri: " +
                        Conjugate(Tense.FutI, Mood.Impv, deponensPrs);
                }
                else
                {
                    ans += "praesentis activi:\t" + Conjugate(Tense.Prs, Mood.Inf);
                    ans += ";\npraesentis passivi:\t" + Conjugate(Tense.Prs, Mood.Inf, true);
                    ans += ";\nperfecti activi:\t" + Conjugate(Tense.Pfct, Mood.Inf);
                    if (SupinStem != "")
                    {
                        ans += ";\nperfecti passivi:\t" + Conjugate(Tense.Pfct, Mood.Inf, true);
                        ans += ";\nfuturi activi:  \t" + Conjugate(Tense.FutI, Mood.Inf);
                        if (NoSupins) ans += "\nSupina non habet.\n";
                        else
                        {
                            ans += ";\nfuturi passivi: \t" + Conjugate(Tense.FutI, Mood.Inf, true);
                            ans += "\nSupinum primum:  \t" + Supinum1;
                            ans += "\nSupinum secundum:\t" + Supinum2 + "\n";
                        }
                        ans += "\nParticipium perfecti passivi:\n" + PPP.Inflect(false);
                        ans += "\nParticipium futuri activi:\n" + PFA.Inflect(false);
                    }
                    else if (PFAStem != "")
                    {
                        ans += ";\nfuturi activi:  \t" + Conjugate(Tense.FutI, Mood.Inf);
                        ans += "\nParticipium futuri activi:\n" + PFA.Inflect(false);
                    }
                    else ans += "\n\n";
                    if (!EndsWith("t") & !EndsWith("tur"))
                        ans += "Participium praesentis activi:\n" + PPA.Inflect(false);
                    ans += "Formae vocis activi:\n";
                    ans += "Praesens indicativi: " +
                        Conjugate(Tense.Prs, Mood.Ind);
                    ans += "\nImperfectum indicativi: " +
                        Conjugate(Tense.Impf, Mood.Ind);
                    ans += "\nFuturum indicativi: " +
                        Conjugate(Tense.FutI, Mood.Ind);
                    ans += "\nPerfectum indicativi: " +
                        Conjugate(Tense.Pfct, Mood.Ind);
                    ans += "\nPlusquamperfectum indicativi: " +
                        Conjugate(Tense.Plpf, Mood.Ind);
                    ans += "\nFuturum secundum indicativi: " +
                        Conjugate(Tense.FutII, Mood.Ind);
                    ans += "\nPraesens coniunctivi: " +
                        Conjugate(Tense.Prs, Mood.Conj);
                    ans += "\nImperfectum coniunctivi: " +
                        Conjugate(Tense.Impf, Mood.Conj);
                    ans += "\nPerfectum coniunctivi: " +
                        Conjugate(Tense.Pfct, Mood.Conj);
                    ans += "\nPlusquamperfectum coniunctivi: " +
                        Conjugate(Tense.Plpf, Mood.Conj);
                    ans += "\nImperativus praesentis: " +
                        Conjugate(Tense.Prs, Mood.Impv);
                    ans += "\nImperativus futuri: " +
                        Conjugate(Tense.FutI, Mood.Impv);
                    ans += '\n';
                    if (SupinStem == "")
                        ans += "Participio perfecti absente perfectum, plusquamperfectum futurumque secundum absunt.\n";
                    ans += "Formae vocis passivi:\n";
                    ans += "Praesens indicativi: " +
                        Conjugate(Tense.Prs, Mood.Ind, true);
                    ans += "\nImperfectum indicativi: " +
                        Conjugate(Tense.Impf, Mood.Ind, true);
                    ans += "\nFuturum indicativi: " +
                        Conjugate(Tense.FutI, Mood.Ind, true);
                    if (SupinStem != "")
                    {
                        ans += "\nPerfectum indicativi: " +
                          Conjugate(Tense.Pfct, Mood.Ind, true);
                        ans += "\nPlusquamperfectum indicativi: " +
                            Conjugate(Tense.Plpf, Mood.Ind, true);
                        ans += "\nFuturum secundum indicativi: " +
                            Conjugate(Tense.FutII, Mood.Ind, true);
                    }
                    ans += "\nPraesens coniunctivi: " +
                        Conjugate(Tense.Prs, Mood.Conj, true);
                    ans += "\nImperfectum coniunctivi: " +
                        Conjugate(Tense.Impf, Mood.Conj, true);
                    if (SupinStem != "")
                    {
                        ans += "\nPerfectum coniunctivi: " +
                            Conjugate(Tense.Pfct, Mood.Conj, true);
                        ans += "\nPlusquamperfectum coniunctivi: " +
                            Conjugate(Tense.Plpf, Mood.Conj, true);
                    }
                    ans += "\nImperativus praesentis: " +
                        Conjugate(Tense.Prs, Mood.Impv, true);
                    ans += "\nImperativus futuri: " +
                        Conjugate(Tense.FutI, Mood.Impv, true);
                }
            }
            return ans.Replace(" \n", "\n") + '\n';
        }
    }
    struct LatinAdj
    {
        bool CapitalFirst; //words stored in toLower
        public bool EndsWith(string s)
        {
            if (Nom.Contains("-")) return Pr.Delen(Nom.Substring(0,
                Nom.IndexOf('-'))).EndsWith(s);
            return Pr.Delen(Nom).EndsWith(s);
        }
        public bool NumerusCardinalisNecUnus
        {
            get { return masc.IsPlT; }
        }
        public bool GreekDecl
        {
            get { return masc.GreekDecl; }
            set
            {
                masc.GreekDecl = true;
                fem.GreekDecl = true;
                neut.GreekDecl = true;
            }
        }
        public bool isIuvenisType
        {
            get { return masc.isIuvenisType; }
            set
            {
                masc.isIuvenisType = true;
                fem.isIuvenisType = true;
                neut.isIuvenisType = true;
            }
        }
        bool ptcp;
        public bool Participium
        {
            get { return ptcp; }
            set { ptcp = value; AdjAcc(); }
        }
        public bool Pronominal
        {
            get { return fem.Pronominal; }
            set
            {
                masc.Pronominal = value;
                fem.Pronominal = value;
                neut.Pronominal = value;
            }
        }
        void AdjAcc()
        {
            masc.isTurrisType = !ptcp;
            fem.isTurrisType = !ptcp;
            neut.isTurrisType = !ptcp;
            if (fem.isTurrisType)
            {
                string rAcc = fem.OneForm(LatinNoun.Case.Gen, false);
                rAcc = rAcc.Substring(0, rAcc.Length - 2) + "em";
                AddIrregularForm(LatinNoun.Case.Acc, LatinNoun.Gend.M, rAcc);
                AddIrregularForm(LatinNoun.Case.Acc, LatinNoun.Gend.F, rAcc);
            }
            if (Pronominal & !Pr.EW(fem.Nominative, "a"))
            {
                AddIrregularForm(LatinNoun.Case.Nom, LatinNoun.Gend.PlN,
                    fem.Nominative);
            }
        }
        string Nom;
        public string Nominative
        {
            get
            {
                if (CapitalFirst)
                    return Nom[0].ToString().ToUpper() + Nom.Substring(1);
                return Nom;
            }
        }
        public string Adverbium;
        public override string ToString()
        {
            string adv = ".\nAdverbium est " + Adverbium;
            if (Adverbium == "") adv = ".\nAdverbium non habet";
            if (masc.IsPlT)
                return Nom + "\nNomen numerale cardinale"
                    + adv + ", de declinatione non dicitur.\n";
            if (Nom != fem.Nominative)
                if (ptcp) return Nom + ", " + fem.Nominative + ", " +
                        neut.Nominative + "\nParticipium.\nDeclinatio: " +
                    Declension + adv + ".\n";
                else return Nom + ", " + fem.Nominative + ", " +
                        neut.Nominative + "\nNomen adiectivum.\nDeclinatio: "
                        + Declension + adv + ".\n";
            if (ptcp) return Nom + "\nParticipium.\nDeclinatio: " +
                    Declension + adv + ".\n";
            if (Nom != neut.Nominative) return Nom + ", " + neut.Nominative
                    + "\nNomen adiectivum.\nDeclinatio: " + Declension +
                    adv + ".\n";
            return Nom + "\nNomen adiectivum.\nDeclinatio: " + Declension
                + adv + ".\n";
        }
        public string Declension
        {
            get
            {
                string ans = fem.Declension;
                if (ans == "1") return "1 et 2"; else return ans;
            }
        }
        LatinNoun masc;
        LatinNoun fem;
        LatinNoun neut;
        public string OneForm(LatinNoun.Case c, LatinNoun.Gend g)
        {
            if (g == LatinNoun.Gend.C) g = LatinNoun.Gend.F;
            bool plural = LatinNoun.IsPluralGender(g);
            switch ((byte)g % 4)
            {
                case 0: return neut.OneForm(c, plural);
                case 2: return fem.OneForm(c, plural);
                default: return masc.OneForm(c, plural);
            }
        }
        public LatinAdj(string s1, string s2, bool PlT = false)
        {
            CapitalFirst = s1[0].ToString().ToLower() != s1[0].ToString();
            Nom = s1.ToLower();
            if (s1.Contains("-"))
                s1 = s1.Substring(0, s1.IndexOf('-'));
            if (s2.Contains("-"))
                s2 = s2.Substring(0, s2.IndexOf('-'));
            string stem = s2.Substring(0, s2.Length - 1);
            if (PlT)
            {
                if (Pr.EW(s1, "es")) //Tres, tria
                {
                    masc = new LatinNoun(s1, stem + "um", LatinNoun.Gend.PlM);
                    fem = new LatinNoun(s1, stem + "um", LatinNoun.Gend.PlF);
                    neut = new LatinNoun(s2, stem + "um", LatinNoun.Gend.PlN);
                    Adverbium = stem + "s";
                }
                else
                {
                    masc = new LatinNoun(s1, s2, LatinNoun.Gend.PlM);
                    fem = new LatinNoun(s1, s2, LatinNoun.Gend.PlF);
                    neut = new LatinNoun(s1, s2, LatinNoun.Gend.PlN);
                    if (Pr.EW(s1, "em") | Pr.EW(s1, "um"))
                    {
                        Adverbium = s1.Substring(0, s1.Length - 2) + "iēs";
                    }
                    else
                    {
                        if (Pr.EW(s2, "a")) Adverbium = stem + "iēs";
                        else Adverbium = s1 + "iēs";
                    }
                }
            }
            if (Pr.EW(s2, "is"))
            {
                masc = new LatinNoun(s1, s2, LatinNoun.Gend.M);
                fem = new LatinNoun(s1, s2, LatinNoun.Gend.F);
                neut = new LatinNoun(s1, s2, LatinNoun.Gend.N);
                stem = s2.Substring(0, s2.Length - 2);
            }
            else
            {
                masc = new LatinNoun(s1, s1, LatinNoun.Gend.M);
                fem = new LatinNoun(s1, s1, LatinNoun.Gend.F);
                neut = new LatinNoun(s2, s1, LatinNoun.Gend.N); //Sic!
            }
            if (stem.EndsWith("nt")) Adverbium = stem + "er";
            else Adverbium = stem + "ĭter";
            ptcp = false;
            Adverbium = Adverbium.ToLower();
            AdjAcc();
            if (Pronominal & (neut.EndsWith("quid") | neut.EndsWith("quod")))
                Adverbium = Adverbium.Substring(0, Adverbium.Length - 1)
                    + "ōmōdō";
        }
        public LatinAdj(string m, string f, string n, bool PlT = false)
        {
            CapitalFirst = m[0].ToString().ToLower() != m[0].ToString();
            Nom = m.ToLower();
            if (m.Contains("-"))
                m = m.Substring(0, m.IndexOf('-'));
            if (f.Contains("-"))
                f = f.Substring(0, f.IndexOf('-'));
            if (n.Contains("-"))
                n = n.Substring(0, n.IndexOf('-'));
            if (PlT)
            {
                //if (Pr.EW(f, "ae"))
                string stem = f.Substring(0, f.Length - 2);
                masc = new LatinNoun(m, stem + "ōrum", LatinNoun.Gend.PlM);
                fem = new LatinNoun(f, stem + "ārum", LatinNoun.Gend.PlF);
                neut = new LatinNoun(n, stem + "ōrum", LatinNoun.Gend.PlN);
                Adverbium = stem + "iēs";
            }
            else
            {
                if (Pr.EW(f, "is"))
                {
                    masc = new LatinNoun(m, f, LatinNoun.Gend.M);
                    fem = new LatinNoun(f, f, LatinNoun.Gend.F);
                    neut = new LatinNoun(n, f, LatinNoun.Gend.N);
                    Adverbium = f.Substring(0, f.Length - 2) + "ĭter";
                }
                else
                {
                    string stem = f.Substring(0, Pr.Delen(f).LastIndexOf('a')); //Cf. haec, quae 
                    masc = new LatinNoun(m, stem + "ī", LatinNoun.Gend.M);
                    fem = new LatinNoun(f, stem + "ae", LatinNoun.Gend.F);
                    neut = new LatinNoun(n, stem + "ī", LatinNoun.Gend.N);
                    Adverbium = stem + "ē";
                    masc.Pronominal = fem.Pronominal;
                    neut.Pronominal = fem.Pronominal;
                }
            }
            ptcp = false;
            Adverbium = Adverbium.ToLower();
            AdjAcc();
            if (Pronominal & (neut.EndsWith("quid") | neut.EndsWith("quod")))
                Adverbium = Adverbium.Substring(0, Adverbium.Length - 1) 
                    + "ōmōdō";
        }
        public void AddIrregularForm(LatinNoun.Case c, LatinNoun.Gend g, string s)
        {
            switch (g)
            {
                case LatinNoun.Gend.C:
                    AddIrregularForm(c, LatinNoun.Gend.M, s);
                    AddIrregularForm(c, LatinNoun.Gend.F, s); break;
                case LatinNoun.Gend.M:
                    masc.AddIrregularForm(c, false, s); break;
                case LatinNoun.Gend.F:
                    fem.AddIrregularForm(c, false, s); break;
                case LatinNoun.Gend.N:
                    neut.AddIrregularForm(c, false, s); break;
                case LatinNoun.Gend.PlM:
                    masc.AddIrregularForm(c, true, s); break;
                case LatinNoun.Gend.PlF:
                    fem.AddIrregularForm(c, true, s); break;
                case LatinNoun.Gend.PlN:
                    neut.AddIrregularForm(c, true, s); break;
            }
        }
        public void AddVariantForm(LatinNoun.Case c, LatinNoun.Gend g, string s)
        {
            switch (g)
            {
                case LatinNoun.Gend.C:
                    AddVariantForm(c, LatinNoun.Gend.M, s);
                    AddVariantForm(c, LatinNoun.Gend.F, s); break;
                case LatinNoun.Gend.PlC:
                    AddVariantForm(c, LatinNoun.Gend.PlM, s);
                    AddVariantForm(c, LatinNoun.Gend.PlF, s); break;
                case LatinNoun.Gend.M:
                    masc.AddVariantForm(c, false, s); break;
                case LatinNoun.Gend.F:
                    fem.AddVariantForm(c, false, s); break;
                case LatinNoun.Gend.N:
                    neut.AddVariantForm(c, false, s); break;
                case LatinNoun.Gend.PlM:
                    masc.AddVariantForm(c, true, s); break;
                case LatinNoun.Gend.PlF:
                    fem.AddVariantForm(c, true, s); break;
                case LatinNoun.Gend.PlN:
                    neut.AddVariantForm(c, true, s); break;
            }
        }
        public void SetLacuna(LatinNoun.Case c, LatinNoun.Gend g)
        {
            switch (g)
            {
                case LatinNoun.Gend.C:
                    SetLacuna(c, LatinNoun.Gend.M);
                    SetLacuna(c, LatinNoun.Gend.F); break;
                case LatinNoun.Gend.PlC:
                    SetLacuna(c, LatinNoun.Gend.PlM);
                    SetLacuna(c, LatinNoun.Gend.PlF); break;
                case LatinNoun.Gend.M:
                    masc.SetLacuna(c, false); break;
                case LatinNoun.Gend.F:
                    fem.SetLacuna(c, false); break;
                case LatinNoun.Gend.N:
                    neut.SetLacuna(c, false); break;
                case LatinNoun.Gend.PlM:
                    masc.SetLacuna(c, true); break;
                case LatinNoun.Gend.PlF:
                    fem.SetLacuna(c, true); break;
                case LatinNoun.Gend.PlN:
                    neut.SetLacuna(c, true); break;
            }
        }
        public string Inflect(bool independent = true)
        {
            string ans = "";
            if (independent) ans = ToString() + '\n';
            ans += "Genere masculino:\n" + masc.Inflect(false);
            ans += "Genere feminino:\n" + fem.Inflect(false);
            ans += "Genere neutro:\n" + neut.Inflect(false);
            return ans;
        }
    }
    struct LatinNoun
    {
        bool CapitalFirst; //words stored in toLower
        public bool EndsWith(string s)
        {
            if (Nom.Contains("-")) return Pr.Delen(Nom.Substring(0,
                Nom.IndexOf('-'))).EndsWith(s);
            return Pr.Delen(Nom).EndsWith(s);
        }
        public enum Gend { N = 0, M = 1, F = 2, C = 3, PlN = 4, PlM = 5, PlF = 6, PlC = 7 };
        public static bool IsPluralGender(Gend g)
        {
            return g >= (Gend)4;
        }
        Gend gender;
        public string Gender
        {
            get
            {
                string s = "";
                if (IsPluralGender(gender)) s = "plurale tantum ";
                s += GenderNames[(int)gender % 4];
                return s;
            }
        }
        public bool IsNeuter
        {
            get
            {
                return (gender == Gend.PlN) | (gender == Gend.N);
            }
        }
        public bool IsPlT
        {
            get
            {
                return IsPluralGender(gender);
            }
        }
        public bool NoPlural;
        string Nom;
        public string Nominative
        {
            get
            {
                if (CapitalFirst)
                    return Nom[0].ToString().ToUpper() + Nom.Substring(1);
                return Nom;
            }
        }
        public enum Decl { Indecl = 0, A = 1, O = 2, I = 3, U = 4, E = 5, TrueI = 6, Cons = 7 };
        public bool GreekDecl;
        Decl decl;
        public bool isTurrisType
        {
            get
            {
                return decl == Decl.TrueI;
            }
            set
            {
                if (value)
                {
                    if (decl == Decl.I) decl = Decl.TrueI;
                }
                else if (decl == Decl.TrueI) decl = Decl.I;
            }
        }
        public bool isIuvenisType
        {
            get
            {
                return decl == Decl.Cons;
            }
            set
            {
                if (value)
                {
                    if (decl == Decl.I) decl = Decl.Cons;
                }
                else if (decl == Decl.Cons) decl = Decl.I;
            }
        }
        public string Declension
        {
            get
            {
                if (Pronominal) return "pronominalis";
                string s = "";
                if (GreekDecl & decl == Decl.Indecl) GreekDecl = false;
                if (GreekDecl) s = " Graeca";
                switch (decl)
                {
                    case Decl.A: s = "1" + s; break;
                    case Decl.O: s = "2" + s; break;
                    case Decl.E: s = "5" + s; break;
                    case Decl.U: s = "4" + s; break;
                    case Decl.TrueI: s = "3 vocalis" + s; break;
                    case Decl.I: s = "3 mixta" + s; break;
                    case Decl.Cons: s = "3 consonantalis" + s; break;
                    default: s = "Indeclinabile"; break;
                }
                return s;
            }
        }
        public bool iLocative;
        bool PronDecl;
        public bool Pronominal
        {
            get
            {
                return PronDecl & ((decl == Decl.A) | (decl == Decl.O)) & !GreekDecl;
            }
            set
            {
                PronDecl = value & ((decl == Decl.A) | (decl == Decl.O)) & !GreekDecl;
                QuiCorrections();
            }
        }
        public override string ToString()
        {
            if (PronDecl | Pr.Delen(Nom) == "ego")
                return Nominative /*+ ", " + OneForm(Case.Gen, false)*/
                    + "\nPronomen.\n";
            if (Declension == "Indeclinabile") return Nominative +
                    "\nNomen substantivum.\nGenus: " + Gender + ".\n"
                    + Declension + ".\n";
            return Nominative + ", " + OneForm(Case.Gen, false)
                + "\nNomen substantivum.\nGenus: " + Gender +
                ".\nDeclinatio: " + Declension + ".\n";
        }
        string Stem;
        public enum Case { Voc = 0, Nom = 1, Gen = 2, Dat = 3, Acc = 4, Abl = 5, Loc = 6 };
        string CaseForm(Case c, bool plural)
        {
            if (NoPlural) plural = false;
            if (IsPlT) plural = true;
            if (plural & ((c == Case.Abl) | (c == Case.Loc))) c = Case.Dat;
            if (plural & (c == Case.Voc)) c = Case.Nom;
            Tuple<Case, bool> form = new Tuple<Case, bool>(c, plural);
            if (irregularForms.ContainsKey(form)) return irregularForms[form];
            if (decl == Decl.Indecl) return Nom;
            if ((!iLocative | plural) & c == Case.Loc) c = Case.Abl;
            if ((c == Case.Acc) & IsNeuter) c = Case.Nom;
            if (c == Case.Voc)
            {
                if ((decl == Decl.O) & (EndsWith("os") |
                    EndsWith("us")) & !(gender == Gend.N))
                {
                    if (EndsWith("ius"))
                        return Nom.Substring(0, Nom.Length - 3) + "ī";
                    return Stem + "ĕ";
                }
                else
                    if (decl == Decl.A & EndsWith("s")) c = Case.Abl;
                    else c = Case.Nom;
            }
            if (!plural & (c == Case.Nom)) return Nom;
            if (decl == Decl.A)
            {
                //if (c == Case.Nom) return Stem + "ae"; //always plural
                if (c == Case.Gen & plural) return Stem + "ārum";
                if (GreekDecl & !plural & (EndsWith("e") | EndsWith("es")))
                {
                    if (c == Case.Gen & (gender == Gend.F)) return Stem + "ēs";
                    if (c == Case.Acc) return Stem + "ēn";
                    if (c == Case.Abl) return Stem + "ē";
                }
                if (c == Case.Gen)
                {
                    if (GreekDecl & (gender == Gend.F)) return Stem + "ās";
                    if (PronDecl) return Stem + "īŭs";
                }
                if (c == Case.Dat)
                {
                    if (plural) return Stem + "īs";
                    if (PronDecl) return Stem + "ī";
                }
                if (c == Case.Acc)
                {
                    if (plural) return Stem + "ās";
                    if (GreekDecl) return Stem + "ān";
                    return Stem + "am";
                }
                if (c == Case.Abl) return Stem + "ā";
                return Stem + "ae";
            }
            if (c == Case.Loc) //Only if sg, iLocative, not 1st decl.
            {
                return Stem + "ī";
            }
            if (decl == Decl.O)
            {
                if (c == Case.Nom) //Always plural
                {
                    if (IsNeuter) return Stem + "ă";
                    if (GreekDecl) return Stem + "oe";
                }
                if (c == Case.Gen)
                {
                    if (plural)
                        if (GreekDecl) return Stem + "ōn";
                        else return Stem + "ōrum";
                    if (PronDecl) return Stem + "īŭs";
                }
                if (c == Case.Acc)
                    if (plural) return Stem + "ōs";
                    else return Stem + "um";
                if ((c == Case.Dat) & plural) return Stem + "īs";
                if ((c == Case.Abl) | ((c == Case.Dat) & !PronDecl))
                    return Stem + "ō";
                return Stem + "ī";
            }
            if (decl == Decl.E)
            {
                if (c == Case.Dat)
                {
                    if (plural) return Stem + "ēbŭs";
                    c = Case.Gen;
                }
                if (c == Case.Abl) return Stem + "ē";
                if (c == Case.Acc & !plural) return Stem + "em";
                if (c == Case.Gen)
                {
                    if (plural) return Stem + "ērum";
                    if (EndsWith("ies")) return Stem + "ēī";
                    return Stem + "eī";
                }
                return Stem + "ēs";
            }
            if ((c == Case.Dat) & plural)
            {
                string tmp = Stem + "ĭbŭs";
                if (decl == Decl.U) tmp += "/" + Stem + "ŭbŭs";
                return tmp;
            }
            if (decl == Decl.U)
            {
                if (c == Case.Nom) //Always plural
                {
                    if (IsNeuter) return Stem + "uă";
                    c = Case.Acc;
                }
                if (c == Case.Gen & plural) return Stem + "uum";
                if (c == Case.Acc & !plural)
                {
                    if (GreekDecl) return Stem + "ūn";
                    return Stem + "um";
                }
                if (!IsNeuter | (c == Case.Gen))
                {
                    if (c == Case.Dat) return Stem + "uī";
                    return Stem + "ūs";
                }
                if (IsNeuter & (c == Case.Dat))
                    return Stem + "ū/" + Stem + "ūī";
                return Stem + "ū";
            }
            if (c == Case.Gen)
            {
                if (!plural)
                {
                    if (GreekDecl & decl == Decl.TrueI) return Stem + "eŏs";
                    return Stem + "ĭs";
                }
                if (decl == Decl.Cons) return Stem + "um";
                return Stem + "ium";
            }
            if (decl == Decl.TrueI)
            {
                if (c == Case.Abl) c = Case.Dat;
                if (c == Case.Acc & !plural)
                {
                    if (GreekDecl) return Stem + "ĭn";
                    return Stem + "im";
                }
            }
            if (c == Case.Acc & !plural)
            {
                if (GreekDecl & decl == Decl.Cons) return Stem + "ă";
                return Stem + "em";
            }
            if (c == Case.Acc & decl == Decl.Cons) c = Case.Nom;
            if (c == Case.Nom) //Always plural
            {
                if (IsNeuter)
                {
                    if (decl == Decl.Cons) return Stem + "ă";
                    return Stem + "iă";
                }
                return Stem + "ēs";
            }
            if ((c == Case.Acc) & plural)
                return Stem + "ēs/" + Stem + "īs";
            if (c == Case.Abl) return Stem + "ĕ";
            return Stem + "ī";
        }
        public string OneForm(Case c, bool plural)
        {
            string s = CaseForm(c, plural);
            if (Pronominal & s.EndsWith("quīŭs"))
            {
                s = s.Substring(0, s.Length - 5) + "cŭiŭs";
            }
            if (Pronominal & s.EndsWith("quī") & (c == Case.Dat))
            {
                s = s.Substring(0, s.Length - 3) + "cuī";
            }
            if (CapitalFirst)
            {
                return s.Substring(0, 1).ToUpper() + s.Substring(1);
            }
            return s;
        }
        Dictionary<Tuple<Case, bool>, string> irregularForms;
        public void AddIrregularForm(Case c, bool plural, string s)
        {
            s = s.ToLower();
            Tuple<Case, bool> form = new Tuple<Case, bool>(c, plural);
            if (!irregularForms.ContainsKey(form)) irregularForms.Add(form, s);
            else {
                if ((irregularForms[form] != s) &
                    !irregularForms[form].StartsWith(s + "/") &
                    !irregularForms[form].Contains("/" + s))
                    if (irregularForms[form] == "") irregularForms[form] = s;
                    else
                        irregularForms[form] = irregularForms[form] + "/" + s;
            }
        }
        public void AddVariantForm(Case c, bool plural, string s)
        {
            Tuple<Case, bool> form = new Tuple<Case, bool>(c, plural);
            if (!irregularForms.ContainsKey(form))
            {
                irregularForms[form] = CaseForm(c, plural);
            }
            AddIrregularForm(c, plural, s);
        }
        public void SetLacuna(Case c, bool plural)
        {
            irregularForms[new Tuple<Case, bool>(c, plural)] = "";
        }
        static bool AutoGreekDeclFinder(string nom, string gen)
        {
            return Pr.EW(gen, "eos") | (Pr.EW(nom, "e") & Pr.EW(gen, "es")) |
                ((Pr.EW(nom, "as") | Pr.EW(nom, "es")) & Pr.EW(gen, "ae")) |
                ((Pr.EW(nom, "os") | Pr.EW(nom, "on")) & Pr.EW(gen, "i"));
        }
        public LatinNoun(string nom, string gen)
        {
            PronDecl = Pr.EW(gen, "ius") & !(Pr.Delen(nom) == Pr.Delen(gen));
            CapitalFirst = nom[0].ToString().ToLower() != nom[0].ToString();
            gender = Gend.N;
            Nom = nom.ToLower();
            if (nom.Contains("-"))
                nom = nom.Substring(0, nom.IndexOf('-'));
            if (gen.Contains("-"))
                gen = gen.Substring(0, gen.IndexOf('-'));
            if (Pr.EW(gen, "um"))
            {
                gender = Gend.PlF; //Major exceptions corrected below
            }
            if (Pr.EW(gen, "orum") & Pr.EW(nom, "i"))
            {
                gender = Gend.PlM;
                Stem = gen.Substring(0, gen.Length - 4);
                decl = Decl.O;
            }
            if (Pr.EW(nom, "a") & !Pr.EW(gen, "ae"))
            {
                gender = Gend.PlN;
            }
            if (Pr.EW(gen, "eos") | Pr.EW(gen, "ae") | Pr.EW(gen, "ei"))
            {
                gender = Gend.F;
            }
            else
            {
                if (Pr.EW(gen, "i"))
                {
                    gender = Gend.M; //Neuter overridden later
                }
                if (Pr.EW(gen, "is"))
                {
                    gender = Gend.F;
                    if (Pr.EW(nom, "nis") | Pr.EW(nom, "cis")
                        | Pr.EW(nom, "guis") | Pr.EW(nom, "o")
                        | Pr.EW(nom, "er") | Pr.EW(nom, "or") |
                        (Pr.EW(nom, "os") &
                        (byLatinKey.toLatinKey(nom) != "so")))
                    {
                        gender = Gend.M;
                    }
                    if (Pr.EW(nom, "es") & gen.Length > nom.Length)
                    {
                        gender = Gend.M;
                        decl = Decl.Cons;
                    }
                }
                if (Pr.EW(nom, "u") | Pr.EW(nom, "e") |
                    Pr.EW(nom, "ar") | Pr.EW(nom, "al") |
                    Pr.EW(nom, "m") | Pr.EW(nom, "c") | Pr.EW(nom, "t")
                    | Pr.EW(nom, "on") | Pr.EW(nom, "en")
                    | (Pr.EW(nom, "us") & Pr.EW(gen, "ris")))
                {
                    gender = Gend.N;
                }
            }
            NoPlural = false;
            iLocative = false;
            GreekDecl = AutoGreekDeclFinder(nom, gen);
            irregularForms = new Dictionary<Tuple<Case, bool>, string>();
            if (Pr.EW(gen, "i"))
            {
                Stem = gen.Substring(0, gen.Length - 1);
                decl = Decl.O;
            }
            else if (Pr.EW(gen, "eos"))
            {
                Stem = gen.Substring(0, gen.Length - 3);
                decl = Decl.TrueI;
            }
            else if (Pr.EW(gen, "rum") & (gen.Length > nom.Length))
            {
                Stem = gen.Substring(0, gen.Length - 4);
                switch (Pr.Delen(gen)[gen.Length - 4])
                {
                    case 'a': decl = Decl.A; break;
                    case 'e': decl = Decl.E; break;
                    default: decl = Decl.O; break;
                }
            }
            else if (Pr.EW(gen, "uum"))
            {
                Stem = gen.Substring(0, gen.Length - 3);
                decl = Decl.U;
            }
            else if (PronDecl)
            {
                if (Pr.EW(nom, "a") | Pr.EW(nom, "ae") | Pr.EW(nom, "aec"))
                    decl = Decl.A;
                else decl = Decl.O;
                Stem = gen.Substring(0, gen.Length - 3);
            }
            else
            {
                decl = Decl.Cons;
                Stem = gen.Substring(0, gen.Length - 2);
                if (Pr.EW(gen, "ium"))
                    if (Pr.EW(nom, "ia")) decl = Decl.TrueI;
                    else decl = Decl.I;
                if (Pr.EW(gen, "ae") | Pr.EW(gen, "es")) decl = Decl.A;
                if (Pr.EW(gen, "us") & !PronDecl) decl = Decl.U;
                if (Pr.EW(gen, "ei")) decl = Decl.E;
                if (Pr.EW(nom, "es"))
                    nom = nom.Substring(0, nom.Length - 2) + "is";
                if ((Pr.Delen(nom) == Pr.Delen(gen))
                    & !Pr.EW(gen, "ae") & !Pr.EW(gen, "us"))
                    if (Pr.EW(gen, "is")) decl = Decl.I;
                    else decl = Decl.Indecl;
                if (!Pr.IsLatinVowel(Stem[Stem.Length - 1]) &
                    !Pr.IsLatinVowel(Stem[Stem.Length - 2]) &
                    (decl == Decl.Cons)) decl = Decl.I;
                if ((Pr.EW(nom, "e") | Pr.EW(nom, "ar")
                    | Pr.EW(nom, "al")) & Pr.EW(gen, "is"))
                    decl = Decl.TrueI;
            }
            Stem = Stem.ToLower();
            Pronominal |= (decl == Decl.A) & !IsPlT & !Pr.EW(nom, "a");
            QuiCorrections();
        }
        public LatinNoun(string nom, string gen, Gend gend)
        {
            PronDecl = Pr.EW(gen, "ius") & !(Pr.Delen(nom) == Pr.Delen(gen));
            CapitalFirst = nom[0].ToString().ToLower() != nom[0].ToString();
            Nom = nom.ToLower();
            if (nom.Contains("-"))
                nom = nom.Substring(0, nom.IndexOf('-'));
            if (gen.Contains("-"))
                gen = gen.Substring(0, gen.IndexOf('-'));
            gender = gend;
            if (Pr.EW(gen, "m"))
            {
                if (gender == Gend.F) gender = Gend.PlF;
                if (gender == Gend.N) gender = Gend.PlN;
                if (gender == Gend.C) gender = Gend.PlC;
                if (gender == Gend.M) gender = Gend.PlM;
            }
            NoPlural = false;
            iLocative = false;
            GreekDecl = AutoGreekDeclFinder(nom, gen);
            irregularForms = new Dictionary<Tuple<Case, bool>, string>();
            if (Pr.EW(gen, "i") & (!Pr.EW(gen, "ei") | !Pr.EW(nom, "es")))
            {
                Stem = gen.Substring(0, gen.Length - 1);
                decl = Decl.O;
            }
            else if (Pr.EW(gen, "eos"))
            {
                Stem = gen.Substring(0, gen.Length - 3);
                decl = Decl.TrueI;
            }
            else if (Pr.EW(gen, "rum") & (gen.Length > nom.Length))
            {
                Stem = gen.Substring(0, gen.Length - 4);
                switch (Pr.Delen(gen)[gen.Length - 4])
                {
                    case 'a': decl = Decl.A; break;
                    case 'e': decl = Decl.E; break;
                    default: decl = Decl.O; break;
                }
            }
            else if (Pr.EW(gen, "uum"))
            {
                Stem = gen.Substring(0, gen.Length - 3);
                decl = Decl.U;
            }
            else if (PronDecl)
            {
                if (Pr.EW(nom, "a") | Pr.EW(nom, "ae") | Pr.EW(nom, "aec"))
                    decl = Decl.A;
                else decl = Decl.O;
                Stem = gen.Substring(0, gen.Length - 3).ToLower();
                if (Pr.EW(Stem, "cu"))
                    Stem = Stem.Substring(0, Stem.Length - 2) + "qu";
            }
            else
            {
                decl = Decl.Cons;
                if (gen.Length > 1)
                    Stem = gen.Substring(0, gen.Length - 2);
                else Stem = gen;
                if (Pr.EW(gen, "ium"))
                    if (Pr.EW(nom, "ia")) decl = Decl.TrueI;
                    else decl = Decl.I;
                if (Pr.EW(gen, "ae") | Pr.EW(gen, "es")) decl = Decl.A;
                if (Pr.EW(gen, "us") & !PronDecl) decl = Decl.U;
                if (Pr.EW(gen, "ei")) decl = Decl.E;
                if (Pr.EW(nom, "es"))
                    nom = nom.Substring(0, nom.Length - 2) + "is";
                if ((Pr.Delen(nom) == Pr.Delen(gen))
                    & !Pr.EW(gen, "ae") & !Pr.EW(gen, "us"))
                    if (Pr.EW(gen, "is")) decl = Decl.I;
                    else decl = Decl.Indecl;
                if (Stem.Length > 2)
                    if (!Pr.IsLatinVowel(Stem[Stem.Length - 1]) &
                    !Pr.IsLatinVowel(Stem[Stem.Length - 2]) &
                    (decl == Decl.Cons)) decl = Decl.I;
                if ((Pr.EW(nom, "e") | Pr.EW(nom, "ar")
                    | Pr.EW(nom, "al")) & Pr.EW(gen, "is"))
                    decl = Decl.TrueI;
            }
            Stem = Stem.ToLower();
            Pronominal |= (decl == Decl.A) & !IsPlT & !Pr.EW(nom, "a");
            QuiCorrections();
        }
        void QuiCorrections()
        {
            string truenom = Nom;
            if (truenom.Contains("-"))
                truenom = truenom.Substring(0, truenom.IndexOf('-'));
            if (truenom == "ego")
            {
                NoPlural = true;
                decl = Decl.E;
                Stem = "m";
                AddIrregularForm(Case.Dat, false, "mĭhĭ");
                AddVariantForm(Case.Dat, false, "mĭhī");
                AddVariantForm(Case.Dat, false, "mī");
                AddIrregularForm(Case.Acc, false, "mē");
            }
            if (Pronominal & (Pr.EW(truenom, "quod") | Pr.EW(truenom, "quae")
                | Pr.EW(truenom, "quis") | Pr.EW(truenom, "qui")))
            {
                AddIrregularForm(Case.Dat, true, truenom.Substring(0,
                    truenom.LastIndexOf('u')) + "uĭbŭs");
                if ((gender == Gend.C) | (gender == Gend.M))
                    AddIrregularForm(Case.Acc, false, truenom.Substring(0, 
                        truenom.LastIndexOf('u')) + "uem");
            }
        }
        public static readonly List<string> CaseNames = new List<string> {
            "Vocativus", "Nominativus", "Genitivus", "Dativus\t",
            "Accusativus", "Ablativus", "Locativus" };
        public static readonly List<string> GenderNames = new List<string>
            { "neutrum", "masculinum", "femininum", "commune" };
        public string Inflect(bool independent = true)
        {
            string ans = "";
            if (independent) ans = ToString() + '\n';
            if (!IsPlT)
            {
                for (int c = 1; c <= 5; c++)
                {
                    ans += CaseNames[c] + "\tsingularis est:\t" + OneForm((Case)c, false) + "\n";
                }
                if (iLocative | irregularForms.ContainsKey(new Tuple<Case, bool>(Case.Loc, false)))
                    ans += "Locativus\tsingularis est:\t" + OneForm(Case.Loc, false) + "\n";
                if (OneForm(Case.Voc, false) != OneForm(Case.Nom, false))
                    ans += "Vocativus\tsingularis est:\t" + OneForm(Case.Voc, false) + "\n";
            }
            if (!NoPlural)
                for (int c = 1; c <= 5; c++)
                {
                    ans += CaseNames[c] + "\tpluralis est:\t" + OneForm((Case)c, true) + "\n";
                }
            return ans + '\n';
        }
    }
    class byLatinKey : Comparer<Entry>
    {
        public static string toGenericKey(string s)
        {
            string ans = "";
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[s.Length - 1 - i];
                if ((c == ')') | (c == '(')) continue;
                ans += c;
            }
            return ans.ToLower();
        }
        public static string LatinBaseFormRules(string s)
        {
            string[] ans = Pr.LatinOrthoSimplifications(s).Split(","
                .ToCharArray());
            for (int i = 0; i < ans.Length; i++)
            {
                if ((Pr.Delen(ans[i]) == "vis") | 
                    (Pr.Delen(ans[i]) == "vir")) continue;
                if (Pr.EW(ans[i], "ve") & (i != 0)) continue;
                if (Pr.EW(ans[i], "ve") | Pr.EW(ans[i], "ne")
                    & (Pr.Delen(ans[i]) != "digne") & (Pr.Delen(ans[i])
                        != "paene") & (Pr.Delen(ans[i]) != "sine") & 
                        (Pr.Delen(ans[i]) != "bene") & (Pr.Delen(ans[i])
                        != "ne"))
                    ans[i] = ans[i].Insert(ans[i].Length - 2, "-");
                if (Pr.EW(ans[i], "que") /*| ans[i].EndsWith("vis")*/)
                    ans[i] = ans[i].Insert(ans[i].Length - 3, "-");
                if (Pr.EW(ans[i], "vir")) //No-break hyphen to cheat morphology
                    ans[i] = ans[i].Insert(ans[i].Length - 3, 
                        '\u2011'.ToString());
            }
            return string.Join(",", ans).Replace("--","-");
        }
        public static string toLatinKey(string s)
        {
            return toGenericKey(Pr.Delen(LatinBaseFormRules(s)));
        }
        public override int Compare(Entry s1, Entry s2)
        {
            int l = toLatinKey(s1.BaseForm).CompareTo(toLatinKey(s2.BaseForm));
            if (l == 0) //os, oris vs. os, ossis; velle vs. volare
                l = toGenericKey(s1.ToString()).CompareTo(toGenericKey(s2.ToString()));
            return l;
        }
    }
    class Pr
    {
        [STAThread]
        internal static bool EW(string s1, string s2)
        {
            return Delen(s1).EndsWith(s2);
        }
        public static bool IsLatinVowel(char c)
        {
            if (c == '\u0304') return false;
            if (c == '\u0306') return false;
            c = Delen(c.ToString()).ToLower()[0];
            if ((c == 'a') | (c == 'e') | (c == 'i') |
                (c == 'o') | (c == 'u') | (c == 'y')) return true;
            return false;
        }
        public static string Delen(string s)
        {
            return s.Replace('\u0304'.ToString(), "") //Macron
                .Replace('\u0306'.ToString(), "") //Breve
                .Replace('ā', 'a').Replace('ă', 'a')
                .Replace('Ā', 'A').Replace('Ă', 'A')
                .Replace('ē', 'e').Replace('ĕ', 'e')
                .Replace('Ē', 'E').Replace('Ĕ', 'E')
                .Replace('ī', 'i').Replace('ĭ', 'i')
                .Replace('Ī', 'I').Replace('Ĭ', 'I')
                .Replace('ō', 'o').Replace('ŏ', 'o')
                .Replace('Ō', 'O').Replace('Ŏ', 'O')
                .Replace('ū', 'u').Replace('ŭ', 'u')
                .Replace('Ū', 'U').Replace('Ŭ', 'U');
        }
        public static string LatinOrthoSimplifications(string s)
        {
            return s.Replace('j', 'i').Replace("æ", "ae").Replace("Æ", "Ae");
        }
        static SortedSet<Entry> UploadEntries(IComparer<Entry> by)
        {
            SortedSet<Entry> dict = new SortedSet<Entry>(by);
            StreamReader sr = new StreamReader(source);
            while (!sr.EndOfStream)
            {
                string curr = sr.ReadLine().Replace('\u00a0', ' ').Trim(); //nbsp -> ' '
                if (curr.Trim("\t|-".ToCharArray()) == "") continue;
                if (!curr.Contains("|")) continue;
                while (curr.Contains("  ")) curr = curr.Replace("  ", " ");
                curr = curr.Replace(", ", ",");
                string[] tmp = curr.Split("\t|".ToCharArray());
                curr = byLatinKey.LatinBaseFormRules(tmp[0].TrimEnd());
                Entry.PoS pos;
                LatinNoun.Gend g = LatinNoun.Gend.N;
                bool hasGender = false;
                bool isPron = false;
                switch (tmp[1])
                {
                    case "c":
                        g = LatinNoun.Gend.C; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "m":
                        g = LatinNoun.Gend.M; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "f":
                        g = LatinNoun.Gend.F; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "n":
                        g = LatinNoun.Gend.N; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "c pl":
                        g = LatinNoun.Gend.PlC; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "m pl":
                        g = LatinNoun.Gend.PlM; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "f pl":
                        g = LatinNoun.Gend.PlF; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "n pl":
                        g = LatinNoun.Gend.PlN; hasGender = true;
                        pos = Entry.PoS.Noun; break;
                    case "s": pos = Entry.PoS.Noun; break;
                    case "subst": pos = Entry.PoS.Noun; break;
                    case "num": pos = Entry.PoS.Num; break;
                    case "a":
                        if (curr.Contains(",")) pos = Entry.PoS.Adj;
                        else pos = Entry.PoS.Indecl; break; //Adverbs
                    case "adj": pos = Entry.PoS.Adj; break;
                    case "v": pos = Entry.PoS.Verb; break;
                    case "adjpron":
                        isPron = true;
                        pos = Entry.PoS.Adj; break;
                    case "apron":
                        isPron = true;
                        if (curr.Contains(",")) pos = Entry.PoS.Adj;
                        else pos = Entry.PoS.Indecl; break;
                    case "advpron":
                        isPron = true;
                        pos = Entry.PoS.Indecl; break;
                    case "pron":
                        isPron = true;
                        if (curr.IndexOf(',') != curr.LastIndexOf(','))
                            pos = Entry.PoS.Adj;
                        else
                        {
                            g = LatinNoun.Gend.C;
                            hasGender = true;
                            pos = Entry.PoS.Noun;
                        }
                        break;
                    case "pron n":
                        isPron = true; g = LatinNoun.Gend.N;
                        hasGender = true; pos = Entry.PoS.Noun; break;
                    default: pos = Entry.PoS.Indecl; break;
                }
                Entry gr;
                //Clear tmp somehow?
                tmp = curr.Split(",".ToCharArray());
                if (hasGender) gr = new Entry(g, tmp);
                else gr = new Entry(pos, tmp);
                gr.Pronominal = isPron;
                if (!dict.Contains(gr)) dict.Add(gr);
            }
            sr.Close();
            return dict;
        }
        static SortedSet<Entry> UploadIrregularities(SortedSet<Entry> dict)
        {
            ClearIrregFile();
            StreamReader sr = new StreamReader(irregfile);
            while (!sr.EndOfStream)
            {
                string curr = sr.ReadLine()/*.Trim()*/;
                //if (curr.IndexOf('|') == curr.LastIndexOf('|')) continue;
                /*if (curr.Substring(curr.IndexOf('|')).IndexOf('|') !=
                    curr.Substring(curr.IndexOf('|')).LastIndexOf('|'))
                    continue;*/
                bool variant = curr.EndsWith("+");
                if (variant) curr = curr.Substring(0, curr.Length - 1);
                string entryline = curr.Substring(0, curr.IndexOf('|'));
                curr = curr.Substring(curr.IndexOf('|') + 1);
                string form = curr.Substring(0, curr.IndexOf('|'));
                curr = curr.Substring(curr.IndexOf('|') + 1);
                while (curr.EndsWith("-")) //Delete empty ,-, NOT for -que
                    curr = curr.Substring(0, curr.Length - 1);
                foreach (Entry e in dict)
                {
                    if (e.isInflectable)
                    {
                        if (e.ToString().Substring(0,
                            e.ToString().IndexOf('\t')) != entryline)
                            continue;
                        if (form == "yessups") e.NoSupins = false;
                        else if (form == "nosups") e.NoSupins = true;
                        else if (form == "intr") e.Intrans = true;
                        else if (form == "tran") e.Intrans = false;
                        else if (form == "iloc") e.iLocative = true;
                        else if (form == "normloc") e.iLocative = false;
                        else if (form.StartsWith("gr")) e.GreekDecl = true;
                        else if (form == "pron") e.Pronominal = true;
                        else if (form == "npron") e.Pronominal = false;
                        else if (form == "nogr") e.GreekDecl = false;
                        else if (form == "3voc") e.isTurrisType = true;
                        else if (form == "3cons") e.isIuvenisType = true;
                        else if (form == "3mixta")
                        {
                            e.isTurrisType = false;
                            e.isIuvenisType = false;
                        }
                        else if (form == "adv")
                            if (variant & (curr != ""))
                                e.Adverbium = e.Adverbium + "/" + curr;
                            else e.Adverbium = curr;
                        else if (form == "sup")
                        {
                            if (variant & (curr != ""))
                                e.SupinStem = e.SupinStem + "-/" + curr;
                            else e.SupinStem = curr;
                            e.NoSupins = false;
                        }
                        else if (form == "pfa")
                        {
                            if (variant & (curr != ""))
                                e.PFAStem = e.PFAStem + "-/" + curr;
                            else e.PFAStem = curr;
                        }
                        else if (e.POS == Entry.PoS.Verb)
                        {
                            LatinVerb.Tense t;
                            switch (form.Substring(0, form.IndexOf(' ')))
                            {
                                case "impf": t = LatinVerb.Tense.Impf; break;
                                case "imperf": t = LatinVerb.Tense.Impf; break;
                                case "fut": t = LatinVerb.Tense.FutI; break;
                                case "fut1": t = LatinVerb.Tense.FutI; break;
                                case "futI": t = LatinVerb.Tense.FutI; break;
                                case "fut2": t = LatinVerb.Tense.FutII; break;
                                case "futpfct": t = LatinVerb.Tense.FutII; break;
                                case "futII": t = LatinVerb.Tense.FutII; break;
                                case "pfct": t = LatinVerb.Tense.Pfct; break;
                                case "pft": t = LatinVerb.Tense.Pfct; break;
                                case "perf": t = LatinVerb.Tense.Pfct; break;
                                case "plpf": t = LatinVerb.Tense.Plpf; break;
                                case "plperf": t = LatinVerb.Tense.Plpf; break;
                                default: t = LatinVerb.Tense.Prs; break;
                            }
                            LatinVerb.Mood m;
                            byte p = 0;
                            if (form.Substring(form.IndexOf(' '))
                                .Contains("1")) p = 1;
                            else if (form.Substring(form.IndexOf(' '))
                                .Contains("2")) p = 2;
                            bool plural = form.Substring(form.IndexOf(' '))
                                .Contains("pl");
                            bool passive = form.Substring(form.
                                LastIndexOf(' ')).StartsWith("pas");
                            if (form.Contains("inf"))
                            {
                                m = LatinVerb.Mood.Inf;
                                p = 0;
                                plural = false;
                            }
                            else
                            {
                                if (form.Contains("con"))
                                    m = LatinVerb.Mood.Conj;
                                else if (form.Substring(form.IndexOf(' ')).
                                    Contains("imp"))
                                    m = LatinVerb.Mood.Impv;
                                else m = LatinVerb.Mood.Ind;
                            }
                            if (curr == "") e.SetVerbalLacuna(t, m, p,
                                plural, passive);
                            else if (variant)
                                e.AddVerbalVariantForm(t, m, p, plural,
                                    passive, curr);
                            else e.AddVerbalIrregularForm(t, m, p,
                                plural, passive, curr);
                        }
                        else
                        {
                            char gender;
                            LatinNoun.Gend g;
                            if (form.StartsWith("pl ")) gender = form[3];
                            else gender = form[0];
                            switch (gender)
                            {
                                case 'm': g = LatinNoun.Gend.M; break;
                                case 'f': g = LatinNoun.Gend.F; break;
                                case 'c': g = LatinNoun.Gend.C; break;
                                default: g = LatinNoun.Gend.N; break;
                            }
                            if (form.Contains("pl") | e.isPlT) g += 4;
                            LatinNoun.Case c;
                            switch (form.Substring(form.IndexOf(',') + 1))
                            {
                                case "voc": c = LatinNoun.Case.Voc; break;
                                case "v": c = LatinNoun.Case.Voc; break;
                                case "loc": c = LatinNoun.Case.Loc; break;
                                case "l": c = LatinNoun.Case.Loc; break;
                                case "acc": c = LatinNoun.Case.Acc; break;
                                case "abl":
                                    if (LatinNoun.IsPluralGender(g))
                                        c = LatinNoun.Case.Dat;
                                    else
                                        c = LatinNoun.Case.Abl; break;
                                case "dat": c = LatinNoun.Case.Dat; break;
                                case "d": c = LatinNoun.Case.Dat; break;
                                case "gen": c = LatinNoun.Case.Gen; break;
                                case "g": c = LatinNoun.Case.Gen; break;
                                default: c = LatinNoun.Case.Nom; break;
                            }
                            if (curr == "") e.SetNominalLacuna(c, g);
                            else if (variant)
                                e.AddNominalVariantForm(c, g, curr);
                            else e.AddNominalIrregularForm(c, g, curr);
                        }
                    }
                }
            }
            sr.Close();
            return dict;
        }
        public static System.Text.Encoding utf = System.Text.Encoding.UTF8;
        static string folder = "C:/Users/dz-zd/Documents/Морфология/";
        public static string source;
        public static string irregfile;
        public static string dictpath;
        static void ClearIrregFile()
        {
			StreamWriter sw;
			if (File.Exists(irregfile))
			{
                List<string> set = new List<string>();
                StreamReader sr = new StreamReader(irregfile);
                while (!sr.EndOfStream)
                {
                    string curr = sr.ReadLine().Trim();
                    if (curr.IndexOf('|') == curr.LastIndexOf('|')) continue;
                    if (set.Contains(curr)) set.Remove(curr);
                    set.Add(curr);
                }
                sr.Close();
                sw = new StreamWriter(irregfile, false, utf);
                if (set.Count > 0)
                {
                    for (int i = 0; i < set.Count - 1; i++)
                    {
                        sw.WriteLine(set[i]);
                    }
                    sw.Write(set[set.Count - 1]);
                }
			}
			else
			{
                sw = new StreamWriter(irregfile, false, utf);
			}
            sw.Close();
            sw.Dispose();
        }
        static void Main(string[] args)
        {
            Console.Title = "Vocabularius grammaticus Latinitatis";
            Console.WriteLine("Hacne directoria vocabularius sit?");
            Console.WriteLine(folder);
            Console.WriteLine("Si sic, \"Enter\" imprime; nisi sic, da mihi directoriam.");
            string s = Console.ReadLine().Trim();
            if (s != "") folder = s;
            source = folder + "latindict_source.txt";
            irregfile = folder + "latindict_irregularities.txt";
            dictpath = folder + "latindict.txt";
            //Console.WriteLine(byLatinKey.toLatinKey(Console.ReadLine()));
            //Console.WriteLine(LatinVerb.esse.Inflect());
            //Console.WriteLine(new LatinVerb("facio", "feci", "factum", "facere").Inflect());
            //Entry facio = new Entry(Entry.PoS.Verb, "facio,feci,factum,facere".Split(",".ToCharArray()));
            byLatinKey byMe = new byLatinKey();
            SortedSet<Entry> dict = UploadIrregularities(UploadEntries(byMe));
            dict.Add(new Entry(LatinVerb.Esse));
            DialogResult irregAdd =
                MessageBox.Show("Visne irregularitates addere?",
                "Irregularitates", MessageBoxButtons.YesNo);
            while (irregAdd == DialogResult.Yes)
            {
                Form1 adder = new Form1(dict);
                adder.ShowDialog();
                irregAdd = 
                    MessageBox.Show("Visne plus irregularitatum addere?",
                "Irregularitates", MessageBoxButtons.YesNo);
            }
            ClearIrregFile();
            StreamWriter sw = new StreamWriter(dictpath, false, utf);
            sw.WriteLine("Vocabularius grammaticus Latinitatis sum.");
            sw.WriteLine("Demetrius Viridianus me fecit.");
            foreach (Entry gr in dict)
            {
                sw.WriteLine("\n---\n");
                sw.WriteLine(gr.Inflect().TrimEnd("\n".ToCharArray()));
            }
            sw.Write("---\n\nFINIS");
            sw.Close();
            sw.Dispose();
            new TxtReader(dictpath).ShowDialog();
            //Console.ReadLine();
        }
    }
}
