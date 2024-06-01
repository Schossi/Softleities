using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Softleitner.Utilities.Editor
{
    public class DependenciesWindow : EditorWindow
    {
        public enum DependencyType { Field, Array, Struct, Event }
        public class Dependency
        {
            public MonoBehaviour Behaviour { get; }
            public DependencyType Type { get; }

            public string Path { get; }
            public string Name { get; }
            public string Component { get; }
            public string Field { get; }

            public Dependency(MonoBehaviour behaviour, string field, DependencyType type)
            {
                Behaviour = behaviour;
                Path = getPath(behaviour.transform);
                Name = behaviour.name;
                Component = Behaviour.GetType().Name;
                Field = field;
                Type = type;
            }
        }

        [MenuItem("CONTEXT/Component/Find Dependencies", secondaryPriority = 16)]
        public static void FindDependencies(MenuCommand command) => GetWindow<DependenciesWindow>(title: "Dependencies").initialize(command.context);

        [SerializeField]
        private UnityEngine.Object _reference;

        private MultiColumnListView _columnListView;
        private List<Dependency> _dependencies;
        private List<Dependency> _dependenciesSorted;

        public void CreateGUI()
        {
            if (_reference != null)
                build();
        }

        private void initialize(UnityEngine.Object reference)
        {
            _reference = reference;
            build();
        }
        private void build()
        {
            rootVisualElement.Clear();

            Func<VisualElement> makeCell = () =>
            {
                var ve = new VisualElement();
                var label = new Label();
                ve.Add(label);

                return ve;
            };

            _columnListView = new MultiColumnListView();
            _columnListView.columns.Add(new Column() { sortable = true, title = "Path", width = new Length(30, LengthUnit.Percent), makeCell = makeCell, bindCell = (e, i) => e.Q<Label>().text = _dependenciesSorted[i].Path });
            _columnListView.columns.Add(new Column() { title = "Name", width = new Length(20, LengthUnit.Percent), makeCell = makeCell, bindCell = (e, i) => e.Q<Label>().text = _dependenciesSorted[i].Name });
            _columnListView.columns.Add(new Column() { title = "Component", width = new Length(20, LengthUnit.Percent), makeCell = makeCell, bindCell = (e, i) => e.Q<Label>().text = _dependenciesSorted[i].Component });
            _columnListView.columns.Add(new Column()
            {
                title = "Field",
                width = new Length(30, LengthUnit.Percent),
                makeCell = makeCell,
                bindCell = (e, i) =>
                {
                    switch (_dependenciesSorted[i].Type)
                    {
                        case DependencyType.Field:
                            e.parent.style.backgroundColor = new Color(0.2f, 0.0f, 0.0f, 0.1f);
                            break;
                        case DependencyType.Array:
                            e.parent.style.backgroundColor = new Color(0.2f, 0.1f, 0.0f, 0.1f);
                            break;
                        case DependencyType.Struct:
                            e.parent.style.backgroundColor = new Color(0.0f, 0.2f, 0.0f, 0.1f);
                            break;
                        case DependencyType.Event:
                            e.parent.style.backgroundColor = new Color(0.0f, 0.0f, 0.2f, 0.1f);
                            break;
                    }

                    e.Q<Label>().text = _dependenciesSorted[i].Field;
                }
            });

            _columnListView.sortingEnabled = true;
            _columnListView.columnSortingChanged += () =>
            {
                sort();
            };

            _dependencies = GetDependencies(_reference).ToList();
            sort();

            _columnListView.selectionChanged += i =>
            {
                if (i.Any())
                    EditorGUIUtility.PingObject(((Dependency)i.First()).Behaviour);
            };
            _columnListView.itemsChosen += i =>
            {
                if (i.Any())
                    Selection.activeObject = ((Dependency)i.First()).Behaviour;
            };

            rootVisualElement.Add(_columnListView);

            var space = new VisualElement();
            space.RegisterCallback<ClickEvent>(e =>
            {
                _columnListView.ClearSelection();
            });
            space.style.flexGrow = 1;
            rootVisualElement.Add(space);

            var label = new Label($"References to {_reference.name}({_reference.GetType()})");
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.paddingLeft = 4;
            label.style.flexGrow = 1f;

            var refresh = new Button(build);
            refresh.Add(new Label("↻"));

            var bar = new VisualElement();
            bar.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            bar.style.flexDirection = FlexDirection.Row;

            bar.Add(label);
            bar.Add(refresh);

            rootVisualElement.Add(bar);
        }
        private void sort()
        {
            if (_columnListView.sortedColumns.Any())
            {
                IOrderedEnumerable<Dependency> sorted = null;

                foreach (var column in _columnListView.sortedColumns)
                {
                    if (sorted == null)
                    {
                        switch (column.columnIndex)
                        {
                            case 0:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = _dependencies.OrderBy(d => d.Path);
                                else
                                    sorted = _dependencies.OrderByDescending(d => d.Path);
                                break;
                            case 1:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = _dependencies.OrderBy(d => d.Name);
                                else
                                    sorted = _dependencies.OrderByDescending(d => d.Name);
                                break;
                            case 2:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = _dependencies.OrderBy(d => d.Component);
                                else
                                    sorted = _dependencies.OrderByDescending(d => d.Component);
                                break;
                            case 3:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = _dependencies.OrderBy(d => d.Field);
                                else
                                    sorted = _dependencies.OrderByDescending(d => d.Field);
                                break;
                        }
                    }
                    else
                    {
                        switch (column.columnIndex)
                        {
                            case 0:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = sorted.ThenBy(d => d.Path);
                                else
                                    sorted = sorted.ThenByDescending(d => d.Path);
                                break;
                            case 1:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = sorted.ThenBy(d => d.Name);
                                else
                                    sorted = sorted.ThenByDescending(d => d.Name);
                                break;
                            case 2:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = sorted.ThenBy(d => d.Component);
                                else
                                    sorted = sorted.ThenByDescending(d => d.Component);
                                break;
                            case 3:
                                if (column.direction == SortDirection.Ascending)
                                    sorted = sorted.ThenBy(d => d.Field);
                                else
                                    sorted = sorted.ThenByDescending(d => d.Field);
                                break;
                        }
                    }
                }

                _dependenciesSorted = sorted.ToList();
            }
            else
            {
                _dependenciesSorted = _dependencies.OrderBy(d => d.Path).ThenBy(d => d.Name).ToList();
            }

            _columnListView.itemsSource = _dependenciesSorted;
        }

        public static IEnumerable<Dependency> GetDependencies(object reference)
        {
            foreach (var behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                foreach (var field in behaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!IsInspectorField(field))
                        continue;

                    object owner = behaviour;

                    if (field.FieldType.IsAssignableFrom(reference.GetType()))
                    {
                        if (field.GetValue(owner) == reference)
                            yield return new(behaviour, field.Name, DependencyType.Field);
                    }
                    else if (field.FieldType.IsArray && field.FieldType.GetElementType().IsAssignableFrom(reference.GetType()))
                    {
                        var array = (Array)field.GetValue(owner);
                        if (array != null)
                        {
                            foreach (var item in array)
                            {
                                if (item == reference)
                                    yield return new(behaviour, field.Name, DependencyType.Array);
                            }
                        }
                    }
                    else if (typeof(UnityEventBase).IsAssignableFrom(field.FieldType))
                    {
                        var e = (UnityEventBase)field.GetValue(owner);
                        for (int i = 0; i < e.GetPersistentEventCount(); i++)
                        {
                            if ((object)e.GetPersistentTarget(i) == reference)
                                yield return new(behaviour, $"{field.Name}({e.GetPersistentMethodName(i)})", DependencyType.Event);
                        }
                    }
                    else if (field.FieldType.IsClass && Attribute.IsDefined(field.FieldType, typeof(SerializableAttribute)))
                    {
                        var path = getReferencePath(field.GetValue(owner), reference);
                        if (path != null)
                            yield return new(behaviour, field.Name + "/" + path, DependencyType.Struct);
                    }

                }
            }
        }
        public static bool IsInspectorField(FieldInfo field)
        {
            if (field.IsPublic && !Attribute.IsDefined(field, typeof(HideInInspector)))
                return true;
            else if (Attribute.IsDefined(field, typeof(SerializeField)))
                return true;
            else
                return false;
        }
        private static string getReferencePath(object owner, object reference)
        {
            if (owner == null)
                return null;

            foreach (var field in owner.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.FieldType.IsAssignableFrom(reference.GetType()) && field.GetValue(owner) == reference)
                    return field.Name;

                if (field.FieldType.IsClass && Attribute.IsDefined(field.FieldType, typeof(SerializableAttribute)) && !field.FieldType.IsAssignableFrom(typeof(UnityEngine.Object)))
                {
                    var path = getReferencePath(field.GetValue(owner), reference);
                    if (path != null)
                        return field.Name + "/" + path;
                }
            }

            return null;
        }
        private static bool containsReference(object owner, object reference)
        {
            if (owner == null)
                return false;

            foreach (var field in owner.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.FieldType.IsAssignableFrom(reference.GetType()) && field.GetValue(owner) == reference)
                    return true;

                if (field.FieldType.IsClass && Attribute.IsDefined(field.FieldType, typeof(SerializableAttribute)) && !field.FieldType.IsAssignableFrom(typeof(UnityEngine.Object)))
                {
                    if (containsReference(field.GetValue(owner), reference))
                        return true;
                }
            }

            return false;
        }

        private static string getPath(Transform transform)
        {
            if (transform.parent == null)
                return string.Empty;

            string path = string.Empty;// "/" + transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}