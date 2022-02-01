import Spy = jasmine.Spy;

export function setProp<T> (spyObj: Spy, prop: string, value: T)
{
  (Object.getOwnPropertyDescriptor(spyObj, prop)?.get as Spy<() => T>).and.returnValue(value);
}
