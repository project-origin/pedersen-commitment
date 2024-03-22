use curve25519_dalek::{
    ristretto::{CompressedRistretto, RistrettoPoint},
    scalar::Scalar,
};
use std::{ptr, slice};

use crate::reref;

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_from_uniform_bytes(
    bytes: *const u8,
) -> *const RistrettoPoint {
    let bytes = slice::from_raw_parts(bytes, 64).try_into().unwrap();
    Box::into_raw(Box::new(RistrettoPoint::from_uniform_bytes(bytes)))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_gut_spill(this: *const RistrettoPoint) {
    let this = reref(this);
    println!("My Guts: {:?}", this.compress());
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_compress(this: *const RistrettoPoint, dst: *mut u8) {
    let this = reref(this);
    let dst = slice::from_raw_parts_mut(dst, 32);
    let src = this.compress().to_bytes();
    dst.clone_from_slice(&src);
}

#[no_mangle]
pub extern "C" fn ristretto_point_decompress(bytes: *const u8) -> *const RistrettoPoint {
    let bytes = unsafe { slice::from_raw_parts(bytes, 32) };
    let Ok(compressed) = CompressedRistretto::from_slice(bytes) else {
        return ptr::null();
    };
    let Some(point) = compressed.decompress() else {
        return ptr::null();
    };
    Box::into_raw(Box::new(point))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_equals(
    lhs: *const RistrettoPoint,
    rhs: *const RistrettoPoint,
) -> bool {
    let lhs = reref(lhs);
    let rhs = reref(rhs);
    lhs == rhs
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_add(
    lhs: *const RistrettoPoint,
    rhs: *const RistrettoPoint,
) -> *const RistrettoPoint {
    let lhs = reref(lhs);
    let rhs = reref(rhs);
    Box::into_raw(Box::new(lhs + rhs))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_sub(
    lhs: *const RistrettoPoint,
    rhs: *const RistrettoPoint,
) -> *const RistrettoPoint {
    let lhs = reref(lhs);
    let rhs = reref(rhs);
    Box::into_raw(Box::new(lhs - rhs))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_negate(this: *const RistrettoPoint) -> *const RistrettoPoint {
    let this = reref(this);
    Box::into_raw(Box::new(-this))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_mul_bytes(
    lhs: *const RistrettoPoint,
    rhs: *const u8,
) -> *const RistrettoPoint {
    let lhs = reref(lhs);

    let rhs = slice::from_raw_parts(rhs, 32);
    let rhs = Scalar::from_bytes_mod_order(rhs.try_into().unwrap());
    Box::into_raw(Box::new(lhs * rhs))
}

#[no_mangle]
pub unsafe extern "C" fn ristretto_point_mul_scalar(
    lhs: *const RistrettoPoint,
    rhs: *const Scalar,
) -> *const RistrettoPoint {
    let lhs = reref(lhs);
    let rhs = reref(rhs);
    Box::into_raw(Box::new(lhs * rhs))
}

#[no_mangle]
pub extern "C" fn ristretto_point_sum(
    args: *const *const RistrettoPoint,
    len: i32,
) -> *const RistrettoPoint {
    let args = unsafe { std::slice::from_raw_parts(args, len as usize) };
    Box::into_raw(Box::new(args.iter().map(|&ptr| unsafe { *ptr }).sum()))
}

#[no_mangle]
pub extern "C" fn ristretto_point_free(this: *mut RistrettoPoint) {
    if this.is_null() {
        return;
    }
    unsafe {
        drop(Box::from_raw(this));
    }
}

#[no_mangle]
pub unsafe extern "C" fn compressed_ristretto_to_bytes(
    this: *mut CompressedRistretto,
    dst: *mut u8,
) {
    let this = reref(this);
    let src = this.as_bytes();
    let dst = slice::from_raw_parts_mut(dst, 32);
    dst.clone_from_slice(src);
}

#[no_mangle]
pub unsafe extern "C" fn compressed_ristretto_from_bytes(
    bytes: *mut u8,
) -> *mut CompressedRistretto {
    let bytes = slice::from_raw_parts(bytes, 32);
    let Ok(point)  = CompressedRistretto::from_slice(bytes) else {
        return ptr::null_mut();
    };
    Box::into_raw(Box::new(point))
}
