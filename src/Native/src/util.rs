#[repr(C)]
#[derive(Debug)]
pub struct RawVec<T> {
    pub data: *mut T,
    pub size: usize,
    pub cap: usize,
}

// This macro aids visually seeing where pointers gets dereferenced into a reference,
// and allows us to possibly 'inject' checks
#[inline(always)]
pub(crate) unsafe fn reref<T>(ptr: *const T) -> &'static T {
    &*ptr
}

#[allow(unused)]
#[inline(always)]
pub(crate) unsafe fn reref_mut<T>(ptr: *mut T) -> &'static mut T {
    &mut *ptr
}
