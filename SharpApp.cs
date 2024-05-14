using Atk;
using Gtk;
using System.IO;
public class SharpApp : Window
{
    private Builder _builder;
    private Window _wnd_glade_main;

    public SharpApp() : base(Gtk.WindowType.Toplevel)
    {
        _builder = new Gtk.Builder();
        _builder.AddFromFile("MyWindow.glade");

        _wnd_glade_main = (Window)_builder.GetObject("MainWindow");
        //     builder.ConnectSignal("signal_name", new EventHandler(HandlerMethod);
        _builder.Autoconnect(this);

        //set the scale
        var scl = (Scale)_builder.GetObject("rng_lighten");
        scl.Adjustment = new Adjustment(0, 0, 99, 1, 10, 10);

        var btn_convert = (Button)_builder.GetObject("btn_convert");
        btn_convert.Clicked += (s, e) =>
        {
            _showDirOutputChooser();
        };

var btn_input_dir = (Button)_builder.GetObject("btn_input_dir");
btn_input_dir.Clicked += (s, e) =>
        {
                    _showSRCDirChooser();
        };



        _wnd_glade_main.ShowAll();

        _cfgTree();

        _wnd_glade_main.DeleteEvent += delegate
        {
            Application.Quit();
        };

    }

    private void _showSrcImage(string image)
    {

        // Define maximum height
        int maxHeight = 480; // Set your desired maximum height here
        var pixbuf = new Gdk.Pixbuf(image);
        // Scale the image down while maintaining aspect ratio
        double scale = (double)maxHeight / pixbuf.Height;
        int newWidth = (int)(pixbuf.Width * scale);
        var scaledPixbuf = pixbuf.ScaleSimple(newWidth, maxHeight, Gdk.InterpType.Bilinear);

        var img_src = (Image)_builder.GetObject("img_src");
        img_src.Pixbuf = scaledPixbuf;
        img_src.WidthRequest = 800;
        img_src.HeightRequest = 480;
    }

    private void _showDestImage(string image, double value)
    {
        var ms = Utilities.Convert(image, value);
        // Define maximum height
        int maxHeight = 480; // Set your desired maximum height here
        var pixbuf = new Gdk.Pixbuf(ms);
        // Scale the image down while maintaining aspect ratio
        double scale = (double)maxHeight / pixbuf.Height;
        int newWidth = (int)(pixbuf.Width * scale);
        var scaledPixbuf = pixbuf.ScaleSimple(newWidth, maxHeight, Gdk.InterpType.Bilinear);

        var img_src = (Image)_builder.GetObject("img_dest");
        img_src.Pixbuf = scaledPixbuf;
        img_src.WidthRequest = 800;
        img_src.HeightRequest = 480;
    }


    private void _showDirOutputChooser()
    {
        var ui_outputdir = (FileChooserDialog)_builder.GetObject("ui_outputdir");
        var home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        ui_outputdir.SetCurrentFolder(home);
        ui_outputdir.Show();
        var btn_outdir_select = (Button)_builder.GetObject("btn_outdir_select");
        btn_outdir_select.Clicked += (o,e)=>
        {
            ui_outputdir.Hide();


            var folder = ui_outputdir.CurrentFolder;
            var scl = (Scale)_builder.GetObject("rng_lighten");

            Task.Run(() =>
            {
                foreach (var file in _files)
                {
                    Utilities.Convert(file, folder, scl.Value);
                }

            });

            _wnd_glade_main.GrabFocus();
        };
    }

    private void _showSRCDirChooser()
    {
        var ui_src_dir = (FileChooserDialog)_builder.GetObject("ui_src_dir");
        var home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        ui_src_dir.SetCurrentFolder(home);
        ui_src_dir.Show();
        var btn_select_dir = (Button)_builder.GetObject("btn_select_dir");
        btn_select_dir.Clicked += delegate
        {
            ui_src_dir.Hide();

            _wnd_glade_main.GrabFocus();

            _changeSelectedDir(ui_src_dir.CurrentFolder);
        };
    }
    static string[] allowedExt = { ".png", ".jpg", ".jpeg", ".bmp" };
    private List<string> _files;

    private void _changeSelectedDir(string f)
    {
        var tv = (TreeView)_builder.GetObject("ui_tree");

        _files = new DirectoryInfo(f)
                    .GetFiles()
                    .Select(f => f.FullName)
                    .Where(f => allowedExt.Contains(System.IO.Path.GetExtension(f.ToLower())))
                    .ToList();


        var store = new ListStore(typeof(string));
        foreach (var fn in _files)
            store.AppendValues(System.IO.Path.GetFileName(fn));
        // store.AppendValues("frank2");


        tv.Model = store;
    }
    private void _cfgTree()
    {
        var tv = (TreeView)_builder.GetObject("ui_tree");
        tv.Selection.Changed += (o, e) =>
        {

            Gtk.TreeIter selected;

            if (tv.Selection.GetSelected(out selected))
            {
                var val = tv.Model.GetValue(selected, 0).ToString();
                //  Console.WriteLine("SELECTED ITEM: {0}", );
                var ui_src_dir = (FileChooserDialog)_builder.GetObject("ui_src_dir");
                var folder = ui_src_dir.CurrentFolder;
                var inputFilePath = System.IO.Path.Combine(folder, val);
                _showSrcImage(inputFilePath);
                var scl = (Scale)_builder.GetObject("rng_lighten");

                _showDestImage(inputFilePath, scl.Value);
                return;
            }

            _showSrcImage(null);
        };
        Gtk.TreeViewColumn tv_fn_col = new Gtk.TreeViewColumn();
        tv_fn_col.Title = "File Name";
        tv.AppendColumn(tv_fn_col);

        Gtk.CellRendererText cellr_fname = new Gtk.CellRendererText();

        // Add the cell to the column
        tv_fn_col.PackStart(cellr_fname, true);
        tv_fn_col.AddAttribute(cellr_fname, "text", 0);

    }

}
