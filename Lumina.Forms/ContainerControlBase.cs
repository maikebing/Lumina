using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Provides a reusable child-control container implementation for LuminaForms controls such as group boxes and panels.
/// </summary>
public abstract class ContainerControlBase : Control
{
    private readonly List<Control> _childControls = [];
    private readonly ControlCollection _controls;

    /// <summary>
    /// Initializes a new container control.
    /// </summary>
    protected ContainerControlBase()
    {
        _controls = new ControlCollection(this);
    }

    /// <summary>
    /// Gets the child controls owned by this container.
    /// </summary>
    public ControlCollection Controls => _controls;

    internal ReadOnlySpan<Control> ChildControls => CollectionsMarshal.AsSpan(_childControls);

    internal void AddChild(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (control.Parent is not null || control.Owner is not null)
        {
            throw new InvalidOperationException("The control already belongs to a container or form.");
        }

        control.SetParent(this);
        _childControls.Add(control);

        if (Owner is not null)
        {
            Owner.AttachControl(control, this);
        }

        if (Handle != 0 && Owner is not null)
        {
            control.CreateHandleRecursive();
        }
    }

    internal void AttachChildrenToOwner(Form owner)
    {
        foreach (Control child in ChildControls)
        {
            owner.AttachControl(child, this);
        }
    }

    internal void CreateChildHandles()
    {
        foreach (Control child in ChildControls)
        {
            child.CreateHandleRecursive();
        }
    }

    /// <summary>
    /// Represents a child-control collection for a LuminaForms container control.
    /// </summary>
    public sealed class ControlCollection : IEnumerable<Control>
    {
        private readonly ContainerControlBase _owner;

        internal ControlCollection(ContainerControlBase owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds a child control to the owning container.
        /// </summary>
        /// <param name="control">The child control to add.</param>
        public void Add(Control control)
        {
            _owner.AddChild(control);
        }

        /// <summary>
        /// Adds a batch of child controls to the owning container.
        /// </summary>
        /// <param name="controls">The child controls to add.</param>
        public void AddRange(IEnumerable<Control> controls)
        {
            ArgumentNullException.ThrowIfNull(controls);

            foreach (Control control in controls)
            {
                _owner.AddChild(control);
            }
        }

        /// <summary>
        /// Adds a batch of child controls to the owning container.
        /// </summary>
        /// <param name="controls">The child controls to add.</param>
        public void AddRange(params Control[] controls)
        {
            AddRange((IEnumerable<Control>)controls);
        }

        /// <summary>
        /// Gets the number of child controls currently owned by the container.
        /// </summary>
        public int Count => _owner._childControls.Count;

        /// <summary>
        /// Gets the child control at the specified index.
        /// </summary>
        /// <param name="index">The zero-based child index.</param>
        /// <returns>The child control.</returns>
        public Control this[int index] => _owner._childControls[index];

        /// <summary>
        /// Determines whether the specified control is already in the owning container.
        /// </summary>
        /// <param name="control">The control to locate.</param>
        /// <returns><see langword="true"/> if the control is present; otherwise, <see langword="false"/>.</returns>
        public bool Contains(Control control)
        {
            return _owner._childControls.Contains(control);
        }

        /// <summary>
        /// Finds controls by <see cref="Control.Name"/> using case-insensitive matching.
        /// </summary>
        /// <param name="key">The control name to locate.</param>
        /// <param name="searchAllChildren"><see langword="true"/> to search nested containers recursively; otherwise, <see langword="false"/>.</param>
        /// <returns>The matching controls.</returns>
        public Control[] Find(string key, bool searchAllChildren)
        {
            ArgumentNullException.ThrowIfNull(key);

            List<Control> matches = [];
            CollectMatches(_owner._childControls, key, searchAllChildren, matches);
            return [.. matches];
        }

        /// <inheritdoc />
        public IEnumerator<Control> GetEnumerator()
        {
            return _owner._childControls.GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void CollectMatches(IEnumerable<Control> controls, string key, bool searchAllChildren, List<Control> matches)
        {
            foreach (Control control in controls)
            {
                if (string.Equals(control.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(control);
                }

                if (searchAllChildren && control is ContainerControlBase container)
                {
                    CollectMatches(container._childControls, key, searchAllChildren, matches);
                }
            }
        }
    }
}
