using System.Collections.Generic;
using System.Windows;
using TSM = Tekla.Structures.Model;
using TSMOP = Tekla.Structures.Model.Operations;
using TSMUI = Tekla.Structures.Model.UI;
using TSM3d = Tekla.Structures.Geometry3d;
using System.Linq;
using System.Collections;
using static Tekla.Structures.Filtering.Categories.PourObjectFilterExpressions;
using System.Windows.Controls.Primitives;

namespace RebarInfoDataTable
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            dataGridView.ItemsSource = TeklaReinforcement.DisplayListOfReinforcement();
        }

        private void dataGridView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TeklaReinforcement.Select();
        }
    }

    internal class TeklaReinforcement
    {
        private static List<TSM.Reinforcement> publicSelectedReinforcement = new List<TSM.Reinforcement>();

        private static List<string> SelectedReinforcementPositionNumbers = new List<string>();
        private static List<string> SelectedReinforcementNames = new List<string>();
        private static List<string> SelectedReinforcementTypes = new List<string>();
        private static List<int> SelectedReinforcementPositionNumbersCounts = new List<int>();

        private static List<mySelectedReinforcemet> ListOfSelectedItemsOnDataTable = new List<mySelectedReinforcemet>();

        public List<mySelectedReinforcemet> MyListOfReinforcement { get; set; }

        private static List<TSM.Reinforcement> GetRebars()
        {
            TSMUI.ModelObjectSelector modelSelector = new TSMUI.ModelObjectSelector();
            TSM.ModelObjectEnumerator selectedObjects = (modelSelector.GetSelectedObjects() as TSM.ModelObjectEnumerator);
            List<TSM.Reinforcement> listOfSelectedReinforcement = new List<TSM.Reinforcement>();
            if (selectedObjects == null)
                return null;
            while (selectedObjects.MoveNext())
            {
                if (!(selectedObjects.Current is TSM.Reinforcement))
                    continue;
                listOfSelectedReinforcement.Add(selectedObjects.Current as TSM.Reinforcement);
            }
            return listOfSelectedReinforcement;
        }

        private static void GetInfoFromReinforcement()
        {
            SelectedReinforcementPositionNumbers.Clear();
            publicSelectedReinforcement = GetRebars();
            if (publicSelectedReinforcement.Count == 0)
                return;
            for(int i = 0; i < publicSelectedReinforcement.Count; i++)
            {
                string posNumber = "?";
                publicSelectedReinforcement[i].GetReportProperty("POS", ref posNumber);
                if (!(SelectedReinforcementPositionNumbers.Contains(posNumber)))
                {
                    SelectedReinforcementPositionNumbers.Add(posNumber);
                    SelectedReinforcementNames.Add(publicSelectedReinforcement[i].Name);
                    SelectedReinforcementPositionNumbersCounts.Add(1);
                    SelectedReinforcementTypes.Add(publicSelectedReinforcement[i].GetType().Name);
                }
                else
                    SelectedReinforcementPositionNumbersCounts[SelectedReinforcementPositionNumbers.Count -1 ]++;
            }
        }

        public static List<mySelectedReinforcemet> DisplayListOfReinforcement()
        {
            GetInfoFromReinforcement();
            List<mySelectedReinforcemet> MyListOfReinforcement = new List<mySelectedReinforcemet>();
            for (int i = 0; i < SelectedReinforcementPositionNumbers.Count; i++)
            {
                mySelectedReinforcemet mySelectedReinforcemet = new mySelectedReinforcemet()
                {
                    Count = SelectedReinforcementPositionNumbersCounts[i],
                    Name = SelectedReinforcementNames[i],
                    Type = SelectedReinforcementTypes[i],
                    PosNumber = SelectedReinforcementPositionNumbers[i]
                };
                MyListOfReinforcement.Add(mySelectedReinforcemet);
            }
            return MyListOfReinforcement;
        }

        private static void GetListOfSelectedItemsOnDataTable()
        {
            var main = (MainWindow)Application.Current.MainWindow;
            if (main.dataGridView.SelectedItems.Count > 0)
            {
                ListOfSelectedItemsOnDataTable.Clear();
                for (int i = 0; i < main.dataGridView.SelectedItems.Count; i++)
                {
                    mySelectedReinforcemet selectedFile = (mySelectedReinforcemet)main.dataGridView.SelectedItems[i];
                    ListOfSelectedItemsOnDataTable.Add(selectedFile);
                }
            }
        }

        private static ArrayList GetListOfReinforcementToSelect()
        {
            ArrayList ObjectsToSelect = new ArrayList();
            GetListOfSelectedItemsOnDataTable();
            for(int i = 0; i < ListOfSelectedItemsOnDataTable.Count; i++)
            {
                for(int j = 0; j < publicSelectedReinforcement.Count; j++)
                {
                    string posNumb = "?";
                    publicSelectedReinforcement[j].GetReportProperty("POS", ref posNumb);
                    if (ListOfSelectedItemsOnDataTable[i].PosNumber == posNumb)
                        ObjectsToSelect.Add(publicSelectedReinforcement[j]);
                }
            }
            return ObjectsToSelect;
        }

        public static void Select()
        {
            ArrayList ObjectsToSelect = GetListOfReinforcementToSelect();
            TSM.Model model = new TSM.Model();
            TSMUI.ModelObjectSelector MS = new TSMUI.ModelObjectSelector();
            MS.Select(ObjectsToSelect);
            model.CommitChanges();
        }
    }
    internal class mySelectedReinforcemet
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PosNumber { get; set; }
    }
}
