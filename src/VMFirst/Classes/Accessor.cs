#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion

using System.Linq.Expressions;
using System.Reflection;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.Classes;

/// <summary>
/// Provides getter / setter functionality for a <see cref="Expression"/>.
/// </summary>
/// <typeparam name="T"> The internal type that the <see cref="Expression"/> encapsulates. </typeparam>
class Accessor<T>
{
	#region Delegates / Events
	#endregion

	#region Constants
	#endregion

	#region Fields

	private readonly Action<T> _setter;

	private readonly Func<T> _getter;

	#endregion

	#region Properties
	#endregion

	#region Enumerations
	#endregion

	#region (De)Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="expression"> The <see cref="Expression"/> that encapsulates either a property or a field for which getter / setter functionality will be provided. </param>
	public Accessor(Expression<Func<T>> expression)
	{
		var memberExpression = (MemberExpression)expression.Body;
		var instanceExpression = memberExpression.Expression;
		var parameter = Expression.Parameter(typeof(T));

		// Build setter and getter method base on the type of the expressions member.
		switch (memberExpression.Member)
		{
			case PropertyInfo propertyInfo:
			{
				_setter = Expression.Lambda<Action<T>>(Expression.Call(instanceExpression, propertyInfo.GetSetMethod(nonPublic: true), parameter), parameter).Compile();
				_getter = Expression.Lambda<Func<T>>(Expression.Call(instanceExpression, propertyInfo.GetGetMethod(nonPublic: true))).Compile();
				break;
			}
			case FieldInfo fieldInfo:
			{
				_setter = Expression.Lambda<Action<T>>(Expression.Assign(memberExpression, parameter), parameter).Compile();
				_getter = Expression.Lambda<Func<T>>(Expression.Field(instanceExpression, fieldInfo)).Compile();
				break;
			}
			default:
			{
				throw new NotImplementedException($"Handling for the type {memberExpression.Member} is not implemented.");
			}
		}
	}

	#endregion

	#region Methods

	public void Set(T value)
	{
		_setter(value);
	}

	public T Get()
	{
		return _getter();
	}

	#endregion
}