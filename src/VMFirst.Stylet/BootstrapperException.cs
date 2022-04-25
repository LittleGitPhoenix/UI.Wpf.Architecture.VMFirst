#region LICENSE NOTICE

//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.

#endregion

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Stylet;

/// <summary>
/// Exception used by the <see cref="StyletBootstrapper{TRootViewModel}"/>.
/// </summary>
public class BootstrapperException : Exception
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message"> <see cref="Exception.Message"/> </param>
	public BootstrapperException(string message) : base(message) { }
}