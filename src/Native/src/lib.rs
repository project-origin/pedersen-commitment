// Since we are providing a C ABI, all unsafety comes from pointer derefs, which all are unsafe
// The unsafe doc is not visible in outside anyway, but the caller should make sure the pointers
// provided are valid with regards to null, alignment, size, type, etc.
#![allow(clippy::missing_safety_doc)]
#![allow(clippy::not_unsafe_ptr_arg_deref)]

use core::slice;

use util::{RawVec, reref};
pub mod generator;
pub mod point;
pub mod rangeproof;
pub mod scalar;
mod util;
pub mod transscript;

#[no_mangle]
pub unsafe extern "C" fn fill_bytes(raw: *const RawVec<u8>, dst: *mut u8) {
    let raw = reref(raw);
    let src = Vec::from_raw_parts(raw.data, raw.size, raw.cap);
    let dst = slice::from_raw_parts_mut(dst, raw.size);
    dst.clone_from_slice(&src);
}

#[no_mangle]
pub unsafe extern "C" fn free_vec(raw: RawVec<u8>) {
    let vec = Vec::from_raw_parts(raw.data, raw.size, raw.cap); // dies here
    drop(vec);
}
